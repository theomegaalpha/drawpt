using DrawPT.Api.AI;
using DrawPT.Api.Cache;
using DrawPT.Api.Hubs;
using DrawPT.Api.Models;
using DrawPT.Api.Repositories;
using DrawPT.Api.Repositories.Models;
using DrawPT.Api.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Channels;

namespace DrawPT.Api.Engine
{
    public class Game
    {
        private readonly TimeSpan _serverTimeout;
        private readonly AIClient _aiClient;
        private IGamePlayer Group { get; set; }
        public CancellationToken Completed => _completedCts.Token;
        private readonly IHubContext<GameHub, IGamePlayer> _hubContext;
        private readonly ReferenceCache _referenceCache;
        private readonly RandomService _randomService;
        private readonly StorageService _storageService;
        private readonly ImageRepository _imageRepository;
        private readonly GameOptions _options;
        private readonly ILogger _logger;

        // Player state keyed by connection id
        private readonly ConcurrentDictionary<string, PlayerState> _players = new();
        // Notification when the game is completed
        private readonly CancellationTokenSource _completedCts = new();
        // Number of open player slots in this game
        private readonly Channel<int> _playerSlots;

        // Game specific props
        private bool _gamblingUnlocked = false;
        public string RoomCode { get; set; }
        public List<GameRound> GameRounds { get; set; } = [];

        private sealed class PlayerState
        {
            public required string ConnectionId { get; init; }
            public Player PlayerObj { get; init; }
            public required IGamePlayer Proxy { get; init; }
            public int Correct { get; set; }
        }

        public Game(IHubContext<GameHub, IGamePlayer> hubContext,
                    ILogger<Game> logger,
                    IOptionsMonitor<GameOptions> options,
                    ReferenceCache referenceCache,
                    RandomService randomService,
                    StorageService storageService,
                    ImageRepository imageRepository,
                    AIClient aiClient)
        {
            _aiClient = aiClient;
            _hubContext = hubContext;
            _referenceCache = referenceCache;
            _randomService = randomService;
            _storageService = storageService;
            _imageRepository = imageRepository;
            _logger = logger;
            _options = options.CurrentValue;
            _playerSlots = Channel.CreateBounded<int>(_options.MaxPlayersPerGame);

            _serverTimeout = TimeSpan.FromSeconds(_options.TimePerQuestion + 8);

            for (int i = 0; i < _options.MaxPlayersPerGame; i++)
                _playerSlots.Writer.TryWrite(0);
        }

        public void SetGroupName()
        {
            Group = _hubContext.Clients.Group(RoomCode);
        }


        /// <summary>
        /// Attempt to add player.   Will fail is room is full.
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public async Task<bool> AddPlayerAsync(string connectionId, Player player)
        {
            if (_playerSlots.Reader.TryRead(out _))
            {
                foreach (var p in _players)
                    await _hubContext.Clients.Client(connectionId).PlayerJoined(p.Value.PlayerObj);

                _players.TryAdd(connectionId, new PlayerState
                {
                    ConnectionId = connectionId,
                    PlayerObj = player,
                    Proxy = _hubContext.Clients.Client(connectionId)
                });

                await _hubContext.Groups.AddToGroupAsync(connectionId, RoomCode);
                await _hubContext.Clients.Group(RoomCode).PlayerJoined(player);
                await _hubContext.Clients.Client(connectionId).SuccessfullyJoined(connectionId);

                // lock room if full
                if (!_playerSlots.Reader.TryPeek(out _))
                    _playerSlots.Writer.TryComplete();

                return true;
            }

            return false;
        }

        public async Task RemovePlayerAsync(string connectionId, Player player)
        {
            if (_players.TryRemove(connectionId, out _))
            {
                // If the game hasn't started (the channel was completed for e.g.), we can give this slot back to the game.
                _playerSlots.Writer.TryWrite(0);

                await Group.PlayerLeft(player);
            }
        }

        public async Task PlayGame()
        {
            int timePerQuestion = _options.TimePerQuestion;
            int timePerThemeSelection = _options.TimePerThemeSelection;

            // The per question cancellation token source
            CancellationTokenSource themeTimoutTokenSource = new();
            CancellationTokenSource questionTimoutTokenSource = new();

            try
            {
                GameConfiguration configuration = new()
                {
                    NumberOfQuestions = _options.QuestionsPerGame,
                    QuestionTimeout = timePerQuestion,
                    ThemeTimeout = timePerThemeSelection
                };

                await Group.GameStarted(configuration);
                List<Task<(PlayerState, GameAnswer)>> playerAnswers = new(_options.MaxPlayersPerGame);

                for (var i = 0; i < _options.QuestionsPerGame; i++)
                {
                    themeTimoutTokenSource.CancelAfter(TimeSpan.FromSeconds(_options.TimePerThemeSelection + 5));

                    // empty game check
                    if (_players.Count == 0)
                        break;

                    var theme = await AskPlayerForTheme(_players.Values.ToList()[i % _players.Count], themeTimoutTokenSource.Token);
                    if (!themeTimoutTokenSource.TryReset())
                        themeTimoutTokenSource = new();
                    await Group.ThemeSelected(theme.Item2.Name);

                    await Task.Delay(5000);

                    playerAnswers.Clear();

                    var gameQuestion = await _aiClient.GenerateGameQuestionAsync(theme.Item2.Name);
                    foreach (var (_, player) in _players)
                        playerAnswers.Add(AskPlayerQuestion(player, i, gameQuestion.ImageUrl, questionTimoutTokenSource.Token));
                    questionTimoutTokenSource.CancelAfter(_serverTimeout);

                    // empty game check
                    if (playerAnswers.Count == 0)
                        break;

                    await Task.WhenAll(playerAnswers);

                    try
                    {
                        await _storageService.DownloadImage(gameQuestion.Id, gameQuestion.ImageUrl);
                        CachedImageEntity cachedImage = new()
                        {
                            Id = gameQuestion.Id,
                            OriginalPrompt = gameQuestion.OriginalPrompt,
                            ThemeId = theme.Item2.Id
                        };
                        await _imageRepository.AddCachedImage(cachedImage);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"{RoomCode}: Error occurred while saving cached image.");
                    }

                    // We were unable to reset so make a new token
                    if (!questionTimoutTokenSource.TryReset())
                        questionTimoutTokenSource = new();

                    var answers = new List<GameAnswer>();
                    foreach (var (player, answer) in playerAnswers.Select(t => t.Result))
                    {
                        answers.Add(answer);
                    }
                    List<GameAnswer> assessedAnswers = new();
                    try
                    {
                        string assessmentJson = await _aiClient.GenerateAssessmentAsync(gameQuestion.OriginalPrompt, answers);
                        assessedAnswers = JsonSerializer.Deserialize<List<GameAnswer>>(assessmentJson) ?? [];
                    }
                    catch
                    {
                        assessedAnswers = new();
                        foreach (var answer in playerAnswers.Select(t => t.Result))
                        {
                            GameAnswer ans = new()
                            {
                                PlayerConnectionId = answer.Item1.ConnectionId,
                                Guess = answer.Item2.Guess,
                                Score = 20,
                                BonusPoints = 5,
                                Reason = "Unfortunately, a server error happened.  You get full points as compensation."
                            };
                            assessedAnswers.Add(ans);
                        }
                    }
                    var gameRound = new GameRound()
                    {
                        Answers = assessedAnswers,
                        Question = gameQuestion,
                        RoundNumber = i
                    };
                    GameRounds.Add(gameRound);
                    await Group.BroadcastRoundResults(gameRound);

                    if (i < _options.QuestionsPerGame - 1)
                        await Group.WriteMessage($"Moving to the next question...");
                    await Task.Delay(_options.TransitionDelay * 1000);
                }

                await Group.WriteMessage("Calculating scores...");
                await Task.Delay(_options.TransitionDelay * 1000);

                try
                {
                    var results = TallyScore();
                    await Group.BroadcastFinalResults(results);
                }
                catch
                {
                    _logger.LogError("Failed to tally and broadcast results.  Most likely room is empty.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"The processing for game {RoomCode} failed unexpectedly");

                await Group.WriteMessage($"The processing for game {RoomCode} failed unexpectedly: {ex}");
            }
            finally
            {
                _logger.LogInformation($"The game {RoomCode} has run to completion.");

                questionTimoutTokenSource.Dispose();

                // Signal that we're done
                _completedCts.Cancel();
            }
        }

        #region game methods

        /// <summary>
        /// Ask the player to select a theme.
        /// </summary>
        /// <param name="playerState"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Tuple of PlayerState and the selected Theme</returns>
        private async Task<(PlayerState, ThemeEntity)> AskPlayerForTheme(PlayerState playerState, CancellationToken cancellationToken)
        {
            List<ThemeEntity> themes = _randomService.GetRandomThemes();
            List<string> themesString = themes.Select(t => t.Name).ToList();
            await _hubContext.Clients.GroupExcept(RoomCode, playerState.ConnectionId).ThemeSelection(themesString);
            try
            {
                string selectedThemeString = await playerState.Proxy.AskTheme(themesString, cancellationToken);
                ThemeEntity selectedTheme = themes.FirstOrDefault(t => t.Name == selectedThemeString) ?? themes.First();
                return (playerState, selectedTheme);
            }
            catch
            {
                // Don't break when answers don't come back successfully.
                return (playerState, themes.First());
            }
        }

        private async Task<(PlayerState, ItemType?)> AskPlayerForItemSelection(PlayerState playerState, CancellationToken cancellationToken)
        {
            List<ItemType> itemTypes = _referenceCache.ItemTypes;
            try
            {
                ItemType? selection = await _hubContext.Clients.Group(RoomCode).AskItemSelection(itemTypes, cancellationToken);
                ItemType? selectedItemType = itemTypes.FirstOrDefault(t => t.Id == selection.Id);
                return (playerState, selectedItemType);
            }
            catch
            {
                // Don't break when answers don't come back successfully.
                return (playerState, itemTypes.First());
            }
        }

        /// <summary>
        /// Ask the player a question until we get a valid answer
        /// </summary>
        /// <param name="playerState"></param>
        /// <param name="roundNumber"></param>
        /// <param name="imageUrl"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<(PlayerState, GameAnswer)> AskPlayerQuestion(PlayerState playerState, int roundNumber, string imageUrl, CancellationToken cancellationToken)
        {
            try
            {
                // Ask the player this question and wait for the response
                Stopwatch stopwatch = new();
                stopwatch.Start();
                GameAnswerBase answer = await playerState.Proxy.AskQuestion(roundNumber, imageUrl, cancellationToken);
                await playerState.Proxy.WriteMessage("Answer received.");

                stopwatch.Stop();
                double elapsedTime = stopwatch.Elapsed.TotalSeconds;
                int timePerQ = _options.TimePerQuestion;
                int bonusPoints = elapsedTime < 10 ? 5 : Math.Max(5 - (int)Math.Round(elapsedTime - 10, 0) / 4, 0);
                if (bonusPoints > 0)
                    await playerState.Proxy.AwardBonusPoints(bonusPoints);
                GameAnswer ans = new()
                {
                    PlayerConnectionId = playerState.ConnectionId,
                    Guess = answer.Guess,
                    Score = 0,
                    BonusPoints = bonusPoints,
                    Reason = "",
                    IsGambling = answer.IsGambling
                };
                return (playerState, ans);
            }
            catch
            {
                GameAnswer ans = new()
                {
                    PlayerConnectionId = playerState.ConnectionId,
                    Guess = "",
                    Score = 0,
                    BonusPoints = 0,
                    Reason = "",
                    IsGambling = false
                };
                // Don't break when answers don't come back successfully.
                return (playerState, ans);
            }
        }

        /// <summary>
        /// Tally the results of all the players at the end of the game.
        /// </summary>
        /// <returns>Collection of GameResults object.</returns>
        private GameResults TallyScore()
        {
            GameResults results = new();
            Dictionary<string, PlayerResult> allPlayerResults = new();
            foreach (var (_, player) in _players)
            {
                PlayerResult newResult = new()
                {
                    Id = player.PlayerObj.Id,
                    ConnectionId = player.ConnectionId,
                    FinalScore = 0,
                    Username = player.PlayerObj.Username
                };
                allPlayerResults[player.ConnectionId] = newResult;
            }

            foreach (var round in GameRounds)
            {
                foreach (var answer in round.Answers)
                {
                    allPlayerResults[answer.PlayerConnectionId].FinalScore += answer.Score + answer.BonusPoints;
                }
            }

            results.PlayerResults = [.. allPlayerResults.Values];
            return results;
        }
        #endregion
    }
}

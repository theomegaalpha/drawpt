using DrawPT.GameEngine.Events;
using DrawPT.GameEngine.Interfaces;
using DrawPT.GameEngine.Models;
using DrawPT.GameEngine.Services;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace DrawPT.GameEngine.Managers
{
    /// <summary>
    /// Manages game rounds and their lifecycle
    /// </summary>
    public class RoundManager : IRoundManager
    {
        private readonly ILogger<RoundManager> _logger;
        private readonly IGameEventBus _eventBus;
        private readonly IConnection _rabbitMqConnection;
        private readonly IModel _channel;
        private readonly string _gameId;
        private readonly GameConfiguration _configuration;
        private readonly IAIService _aiService;
        private readonly IStorageService _storageService;
        private readonly IImageRepository _imageRepository;
        private readonly List<GameRound> _rounds = new();
        private int _currentRoundIndex = -1;

        public RoundManager(
            ILogger<RoundManager> logger,
            IGameEventBus eventBus,
            IConnection rabbitMqConnection,
            string gameId,
            GameConfiguration configuration,
            IAIService aiService,
            IStorageService storageService,
            IImageRepository imageRepository)
        {
            _logger = logger;
            _eventBus = eventBus;
            _rabbitMqConnection = rabbitMqConnection;
            _gameId = gameId;
            _configuration = configuration;
            _aiService = aiService;
            _storageService = storageService;
            _imageRepository = imageRepository;

            // Set up RabbitMQ channel
            _channel = _rabbitMqConnection.CreateModel();
            _channel.ExchangeDeclare("game_events", ExchangeType.Topic, true);

            // Declare queue for round events
            _channel.QueueDeclare($"round_events_{gameId}", true, false, false);
            _channel.QueueBind($"round_events_{gameId}", "game_events", $"game.{gameId}.round.*");
        }

        /// <summary>
        /// Gets the current round
        /// </summary>
        public GameRound? CurrentRound => GetCurrentRound();

        /// <summary>
        /// Gets the current round number
        /// </summary>
        public int CurrentRoundNumber => _currentRoundIndex + 1;

        /// <summary>
        /// Gets all rounds in the game
        /// </summary>
        public IReadOnlyList<GameRound> Rounds => _rounds.AsReadOnly();

        /// <summary>
        /// Starts a new round in the game
        /// </summary>
        public async Task<GameRound> StartNewRoundAsync()
        {
            if (_currentRoundIndex >= _configuration.NumberOfQuestions - 1)
            {
                throw new InvalidOperationException("Maximum number of rounds reached");
            }

            _currentRoundIndex++;

            try
            {
                // Generate a new question for the round
                var gameQuestion = await _aiService.GenerateGameQuestionAsync();
                
                // Create the round
                var round = new GameRound
                {
                    RoundNumber = _currentRoundIndex,
                    Question = gameQuestion,
                    Answers = new List<GameAnswer>()
                };

                _rounds.Add(round);

                // Publish round started event
                var message = new
                {
                    EventType = "RoundStarted",
                    GameId = _gameId,
                    Round = round
                };

                var body = System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(message));
                _channel.BasicPublish(
                    exchange: "game_events",
                    routingKey: $"game.{_gameId}.round.started",
                    basicProperties: null,
                    body: body);

                _logger.LogInformation($"Started round {_currentRoundIndex + 1} in game {_gameId}");

                return round;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to start round {_currentRoundIndex + 1} in game {_gameId}");
                throw;
            }
        }

        /// <summary>
        /// Submits an answer for the current round
        /// </summary>
        public async Task SubmitAnswerAsync(string playerId, string answer, bool isGambling = false)
        {
            if (_currentRoundIndex < 0 || _currentRoundIndex >= _rounds.Count)
            {
                throw new InvalidOperationException("No active round");
            }

            var round = _rounds[_currentRoundIndex];
            var gameAnswer = new GameAnswer
            {
                PlayerConnectionId = playerId,
                Guess = answer,
                IsGambling = isGambling
            };

            round.Answers.Add(gameAnswer);

            // Publish answer submitted event
            var message = new
            {
                EventType = "AnswerSubmitted",
                GameId = _gameId,
                RoundNumber = _currentRoundIndex,
                Answer = gameAnswer
            };

            var body = System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(message));
            _channel.BasicPublish(
                exchange: "game_events",
                routingKey: $"game.{_gameId}.round.answer",
                basicProperties: null,
                body: body);

            _logger.LogInformation($"Player {playerId} submitted answer for round {_currentRoundIndex + 1} in game {_gameId}");
        }

        /// <summary>
        /// Ends the current round and assesses all answers
        /// </summary>
        public async Task EndRoundAsync()
        {
            if (_currentRoundIndex < 0 || _currentRoundIndex >= _rounds.Count)
            {
                throw new InvalidOperationException("No active round");
            }

            var round = _rounds[_currentRoundIndex];

            try
            {
                // Assess all answers
                var assessedAnswers = await _aiService.AssessAnswersAsync(
                    round.Question.OriginalPrompt,
                    round.Answers);

                round.Answers = assessedAnswers;

                // Save the image to storage
                await _storageService.SaveImageAsync(
                    round.Question.Id,
                    round.Question.ImageUrl);

                // Cache the image information
                await _imageRepository.CacheImageAsync(new CachedImage
                {
                    Id = round.Question.Id,
                    OriginalPrompt = round.Question.OriginalPrompt,
                    ThemeId = round.Question.ThemeId
                });

                // Publish round ended event
                var message = new
                {
                    EventType = "RoundEnded",
                    GameId = _gameId,
                    Round = round
                };

                var body = System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(message));
                _channel.BasicPublish(
                    exchange: "game_events",
                    routingKey: $"game.{_gameId}.round.ended",
                    basicProperties: null,
                    body: body);

                _logger.LogInformation($"Ended round {_currentRoundIndex + 1} in game {_gameId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to end round {_currentRoundIndex + 1} in game {_gameId}");
                throw;
            }
        }

        /// <summary>
        /// Gets the current round
        /// </summary>
        private GameRound? GetCurrentRound()
        {
            return _currentRoundIndex >= 0 && _currentRoundIndex < _rounds.Count
                ? _rounds[_currentRoundIndex]
                : null;
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
} 
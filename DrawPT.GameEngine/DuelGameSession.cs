using DrawPT.Common.ServiceBus;
using DrawPT.Common.Interfaces;
using DrawPT.Common.Models;
using DrawPT.Common.Models.Game;
using DrawPT.GameEngine.Interfaces;

namespace DrawPT.GameEngine;

public class DuelGameSession : IGameSession
{
    private readonly IGameCommunicationService _gameCommunicationService;
    private readonly IGameAnnouncerService _announcerService;
    private readonly IAssessmentService _assessmentService;
    private readonly IGameStateService _gameStateService;
    private readonly IQuestionService _questionService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<DuelGameSession> _logger;

    public DuelGameSession(
        ICacheService cacheService,
        IQuestionService questionService,
        IGameStateService gameStateService,
        IAssessmentService assessmentService,
        IGameAnnouncerService gameAnnouncerService,
        IGameCommunicationService gameCommunicationService,
        ILogger<DuelGameSession> logger)
    {
        _cacheService = cacheService;
        _gameStateService = gameStateService;
        _announcerService = gameAnnouncerService;
        _gameCommunicationService = gameCommunicationService;
        _questionService = questionService;
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public async Task PlayGameAsync(string roomCode)
    {
        _logger.LogInformation($"Starting game session for room {roomCode}");

        // Broadcast start game message
        var gameState = await _gameStateService.StartGameAsync(roomCode);

        var promptTimeout = gameState.GameConfiguration.PromptTimeout;
        var questionTimeout = gameState.GameConfiguration.QuestionTimeout;

        List<Player> originalPlayers = await _cacheService.GetRoomPlayersAsync(roomCode);
        await _gameCommunicationService.BroadcastGameEventAsync(roomCode, GameEngineQueue.GameStartedAction, gameState);
        var greetingAnnouncement = await _announcerService.GenerateGreetingAnnouncement(originalPlayers);
        if (greetingAnnouncement != null)
            await _gameCommunicationService.BroadcastGameEventAsync(roomCode, GameEngineQueue.AnnouncerAction, greetingAnnouncement);
        await Task.Delay(35 * 1000);

        List<RoundResults> allRoundResults = new();
        var roundNumber = 0;
        var totalRounds = Math.Ceiling((double)gameState.GameConfiguration.TotalRounds / originalPlayers.Count);
        for (int i = 0; i < totalRounds; i++)
        {
            await _gameCommunicationService.BroadcastGameEventAsync(roomCode, GameEngineQueue.RoundStartedAction, i + 1);
            await Task.Delay(500);

            // ask players for drawing prompts
            var players = await _cacheService.GetRoomPlayersAsync(roomCode);

            // empty game check
            if (players.Count == 0)
                break;

            gameState = await _gameStateService.AskImagePromptAsync(roomCode);

            /* 
             * Ask players for their drawing prompts.
             */
            List<Task<PlayerPrompt>> imagePrompts = new();
            foreach (var player in originalPlayers)
                imagePrompts.Add(_gameCommunicationService.AskPlayerImagePromptAsync(player, promptTimeout));
            gameState = await _gameStateService.AnswerImagePromptAsync(roomCode);

            // generate all images
            await Task.WhenAll(imagePrompts);
            List<Task<GameQuestion>> gameQuestions = new();
            foreach (var imagePrompt in imagePrompts.Select(t => t.Result))
                gameQuestions.Add(_questionService.GenerateQuestionFromPromptAsync(imagePrompt));
            await Task.WhenAll(gameQuestions);

            /* 
             * Loop through game rounds
             */
            foreach (var question in gameQuestions.Select(t => t.Result))
            {
                roundNumber++;
                gameState = await _gameStateService.StartRoundAsync(roomCode, roundNumber);
                List<Task<PlayerAnswer>> playerAnswers = new(players.Count);
                gameState = await _gameStateService.AskQuestionAsync(roomCode);
                foreach (var player in players.Where(p => !question.PlayerGenerated || p.Id != question.PlayerId))
                    playerAnswers.Add(_gameCommunicationService.AskPlayerQuestionAsync(player, question, questionTimeout));

                // empty game check
                if (playerAnswers.Count == 0)
                    break;

                /*
                 * If question generation was successful, ask player for gamble
                 */
                // TODO: handle case where question generation fails
                GameGamble? gamble = null;
                var gambler = players.FirstOrDefault(p => p.Id == question.PlayerId);
                if (gambler == null)
                    _logger.LogError($"Gambler for question {question.Id} not found in room {roomCode}");
                else
                    gamble = await _gameCommunicationService.AskPlayerGambleAsync(gambler, question, questionTimeout);

                await Task.WhenAll(playerAnswers);

                var answers = new List<PlayerAnswer>();
                foreach (var answer in playerAnswers.Select(t => t.Result))
                    answers.Add(answer);

                await _gameCommunicationService.BroadcastGameEventAsync(roomCode, GameEngineQueue.AssessingAnswersAction);

                var assessedAnswers = await _assessmentService.AssessAnswersAsync(question.OriginalPrompt, answers);
                var roundResults = new RoundResults
                {
                    RoundNumber = roundNumber,
                    Question = question,
                    Answers = assessedAnswers
                };
                allRoundResults.Add(roundResults);

                var announcerMessage = await _announcerService.GenerateRoundResultAnnouncement(question.OriginalPrompt, roundResults);
                if (announcerMessage != null)
                    await _gameCommunicationService.BroadcastGameEventAsync(roomCode, GameEngineQueue.AnnouncerAction, announcerMessage);

                await _gameCommunicationService.BroadcastGameEventAsync(roomCode, GameEngineQueue.RoundResultsAction, roundResults);
                await Task.Delay(gameState.GameConfiguration.TransitionDelay * 1000);

                if (gamble != null)
                {
                    gamble = ProcessDuelGameGamble(gamble, assessedAnswers);
                    var gambleMessage = await _announcerService.GenerateGambleResultAnnouncement(gamble);
                    await _gameCommunicationService.BroadcastGameEventAsync(roomCode, GameEngineQueue.GambleResultsAction, gambleMessage);
                    await _gameCommunicationService.BroadcastGameEventAsync(roomCode, GameEngineQueue.AnnouncerAction, gambleMessage);
                }
            }
        }

        var finalGameState = await _gameStateService.EndGameAsync(roomCode);

        var allAnswers = allRoundResults.SelectMany(r => r.Answers);
        var playerScores = allAnswers
            .GroupBy(a => a.PlayerId)
            .ToDictionary(
                g => g.Key,
                g => new PlayerResults
                {
                    PlayerId = g.Key,
                    Score = g.Sum(a => a.Score + a.BonusPoints),
                    Username = originalPlayers.FirstOrDefault(p => p.Id == g.Key)?.Username ?? "Unknown",
                    Avatar = originalPlayers.FirstOrDefault(p => p.Id == g.Key)?.Avatar
                }
            );
        var finalScores = new GameResults
        {
            PlayerResults = playerScores.Values.ToList(),
            WasCompleted = true,
            TotalRounds = gameState.GameConfiguration.TotalRounds
        };
        await _gameCommunicationService.BroadcastGameEventAsync(roomCode, GameEngineQueue.GameResultsAction, finalScores);

        var finalAnnouncement = await _announcerService.GenerateGameResultsAnnouncement(finalScores.PlayerResults);
        if (finalAnnouncement != null)
            await _gameCommunicationService.BroadcastGameEventAsync(roomCode, GameEngineQueue.AnnouncerAction, finalAnnouncement);
        await Task.Delay(gameState.GameConfiguration.TransitionDelay * 1000);

        _logger.LogInformation($"Final scores for room {roomCode}: {string.Join(", ", finalScores.PlayerResults.Select(pr => $"{pr.Username}: {pr.Score}"))}");
    }

    public GameGamble ProcessDuelGameGamble(GameGamble gamble, List<PlayerAnswer> answers)
    {
        PlayerAnswer winningBet;
        if (gamble.IsHigh && answers.Any(a => a.PlayerId == gamble.PlayerId && (a.Score + a.BonusPoints) >= 60))
            gamble.Score = 50;
        else if (!gamble.IsHigh && answers.Any(a => a.PlayerId == gamble.PlayerId && (a.Score + a.BonusPoints) <= 60))
            gamble.Score = 50;

        gamble.Score = answers.Sum(a => a.Score);
        return gamble;
    }

    public GameGamble ProcessGameGamble(GameGamble gamble, List<PlayerAnswer> answers)
    {
        PlayerAnswer winningBet;
        if (gamble.IsHigh)
        {
            var highest = answers.OrderByDescending(a => a.Score).First();
            if (highest.PlayerId == gamble.PlayerId)
                gamble.Score = 50;
        }
        else
        {
            var highest = answers.OrderBy(a => a.Score).First();
            if (highest.PlayerId == gamble.PlayerId)
                gamble.Score = 50;
        }

        gamble.Score = answers.Sum(a => a.Score);
        return gamble;
    }
}

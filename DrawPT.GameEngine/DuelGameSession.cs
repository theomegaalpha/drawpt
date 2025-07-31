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

        List<Player> originalPlayers = await _cacheService.GetRoomPlayersAsync(roomCode);
        await _gameCommunicationService.BroadcastGameEventAsync(roomCode, GameEngineQueue.GameStartedAction, gameState);
        var greetingAnnouncement = await _announcerService.GenerateGreetingAnnouncement(originalPlayers);
        if (greetingAnnouncement != null)
            await _gameCommunicationService.BroadcastGameEventAsync(roomCode, GameEngineQueue.AnnouncerAction, greetingAnnouncement);
        await Task.Delay(35 * 1000);

        List<RoundResults> allRoundResults = new();
        var roundNumber = 1;
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
            List<Task<string>> imagePrompts = new();
            foreach (var player in players.Where(p => !originalPlayers.Any(op => op.Id == p.Id)))
            {
                originalPlayers.Add(player);
                imagePrompts.Add(_gameCommunicationService.AskPlayerImagePromptAsync(player, 30));
            }
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
                gameState = await _gameStateService.StartRoundAsync(roomCode, roundNumber);
                List<Task<PlayerAnswer>> playerAnswers = new(players.Count);
                gameState = await _gameStateService.AskQuestionAsync(roomCode);
                foreach (var player in players)
                    playerAnswers.Add(_gameCommunicationService.AskPlayerQuestionAsync(player, question, 30));

                // empty game check
                if (playerAnswers.Count == 0)
                    break;

                await Task.WhenAll(playerAnswers);

                var answers = new List<PlayerAnswer>();
                foreach (var answer in playerAnswers.Select(t => t.Result))
                    answers.Add(answer);


                await _gameCommunicationService.BroadcastGameEventAsync(roomCode, GameEngineQueue.AssessingAnswersAction);

                var assessedAnswers = await _assessmentService.AssessAnswersAsync(question.OriginalPrompt, answers);
                var roundResults = new RoundResults
                {
                    RoundNumber = i + 1,
                    Question = question,
                    Answers = assessedAnswers
                };
                allRoundResults.Add(roundResults);


                var announcerMessage = await _announcerService.GenerateRoundResultAnnouncement(question.OriginalPrompt, roundResults);
                if (announcerMessage != null)
                    await _gameCommunicationService.BroadcastGameEventAsync(roomCode, GameEngineQueue.AnnouncerAction, announcerMessage);

                await _gameCommunicationService.BroadcastGameEventAsync(roomCode, GameEngineQueue.RoundResultsAction, roundResults);
                await Task.Delay(gameState.GameConfiguration.TransitionDelay * 1000);
                roundNumber++;
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
}

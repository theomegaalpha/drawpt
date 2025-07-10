using DrawPT.Common.ServiceBus;
using DrawPT.Common.Interfaces;
using DrawPT.Common.Models;
using DrawPT.Common.Models.Game;
using DrawPT.Data.Repositories;
using DrawPT.GameEngine.Interfaces;

namespace DrawPT.GameEngine;

public class GameSession : IGameSession
{
    private readonly IGameCommunicationService _gameCommunicationService;
    private readonly IGameAnnouncerService _announcerService;
    private readonly IAssessmentService _assessmentService;
    private readonly IGameStateService _gameStateService;
    private readonly IQuestionService _questionService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GameSession> _logger;

    public GameSession(
        ICacheService cacheService,
        IQuestionService questionService,
        IGameStateService gameStateService,
        IAssessmentService assessmentService,
        IGameAnnouncerService gameAnnouncerService,
        IGameCommunicationService gameCommunicationService,
        ILogger<GameSession> logger)
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
        // Broadcast start game message
        var gameState = await _gameStateService.StartGameAsync(roomCode);

        List<Player> originalPlayers = await _cacheService.GetRoomPlayersAsync(roomCode);
        var greetingAnnouncement = await _announcerService.GenerateGreetingAnnouncement(originalPlayers);
        if (greetingAnnouncement != null)
            _gameCommunicationService.BroadcastGameEvent(roomCode, GameEngineQueue.AnnouncerAction, greetingAnnouncement);
        await Task.Delay(gameState.GameConfiguration.TransitionDelay * 1000);

        _gameCommunicationService.BroadcastGameEvent(roomCode, GameEngineQueue.GameStartedAction, gameState);
        await Task.Delay(100);

        List<RoundResults> allRoundResults = new();

        for (int i = 0; i < gameState.TotalRounds; i++)
        {
            gameState = await _gameStateService.StartRoundAsync(roomCode, i + 1);
            _gameCommunicationService.BroadcastGameEvent(roomCode, GameEngineQueue.RoundStartedAction, i + 1);
            await Task.Delay(500);

            // ask player for theme
            var players = await _cacheService.GetRoomPlayersAsync(roomCode);

            // empty game check
            if (players.Count == 0)
                break;

            // add players that are missing from original list into originalPlayers
            foreach (var player in players.Where(p => !originalPlayers.Any(op => op.Id == p.Id)))
                originalPlayers.Add(player);
            var selectedTheme = await _gameCommunicationService.AskPlayerThemeAsync(players.ElementAt(i % players.Count), 30);
            _gameCommunicationService.BroadcastGameEvent(roomCode, GameEngineQueue.PlayerThemeSelectedAction, selectedTheme);

            // ask all players for their answers
            var question = await _questionService.GenerateQuestionAsync(selectedTheme);
            question.RoundNumber = i + 1;
            List<Task<PlayerAnswer>> playerAnswers = new(players.Count);
            foreach (var player in players)
            {
                playerAnswers.Add(_gameCommunicationService.AskPlayerQuestionAsync(player, question, 30));
            }

            // empty game check
            if (playerAnswers.Count == 0)
                break;

            await Task.WhenAll(playerAnswers);

            var answers = new List<PlayerAnswer>();
            foreach (var answer in playerAnswers.Select(t => t.Result))
                answers.Add(answer);


            _gameCommunicationService.BroadcastGameEvent(roomCode, GameEngineQueue.AssessingAnswersAction);

            var assessedAnswers = await _assessmentService.AssessAnswersAsync(question.OriginalPrompt, answers);
            var roundResults = new RoundResults
            {
                RoundNumber = i + 1,
                Theme = selectedTheme,
                Question = question,
                Answers = assessedAnswers
            };
            allRoundResults.Add(roundResults);


            var announcerMessage = await _announcerService.GenerateRoundResultAnnouncement(question.OriginalPrompt, roundResults);
            if (announcerMessage != null)
                _gameCommunicationService.BroadcastGameEvent(roomCode, GameEngineQueue.AnnouncerAction, announcerMessage);

            _gameCommunicationService.BroadcastGameEvent(roomCode, GameEngineQueue.RoundResultsAction, roundResults);
            await Task.Delay(gameState.GameConfiguration.TransitionDelay * 1000);
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
            TotalRounds = gameState.TotalRounds
        };
        _gameCommunicationService.BroadcastGameEvent(roomCode, GameEngineQueue.GameResultsAction, finalScores);

        var finalAnnouncement = await _announcerService.GenerateGameResultsAnnouncement(finalScores.PlayerResults);
        if (finalAnnouncement != null)
            _gameCommunicationService.BroadcastGameEvent(roomCode, GameEngineQueue.AnnouncerAction, finalAnnouncement);
        await Task.Delay(gameState.GameConfiguration.TransitionDelay * 1000);
    }
}

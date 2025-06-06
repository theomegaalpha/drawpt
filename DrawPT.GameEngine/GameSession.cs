using DrawPT.Common.Configuration;
using DrawPT.Common.Interfaces;
using DrawPT.Common.Models.Game;
using DrawPT.GameEngine.Interfaces;
using System.Text.Json;

namespace DrawPT.GameEngine;

public class GameSession : IGameSession
{
    private readonly IGameCommunicationService _gameCommunicationService;
    private readonly IGameStateService _gameStateService;
    private readonly IQuestionService _questionService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GameSession> _logger;

    public GameSession(
        ICacheService cacheService,
        IQuestionService questionService,
        IGameStateService gameStateService,
        IGameCommunicationService gameCommunicationService,
        ILogger<GameSession> logger)
    {
        _cacheService = cacheService;
        _gameStateService = gameStateService;
        _gameCommunicationService = gameCommunicationService;
        _questionService = questionService;
        _logger = logger;
    }

    public async Task PlayGameAsync(string roomCode)
    {
        // Broadcast start game message
        var gameState = await _gameStateService.StartGameAsync(roomCode);
        _gameCommunicationService.BroadcastGameEvent(roomCode, GameEngineBroadcastMQ.GameStartedAction, gameState);
        await Task.Delay(100);

        for (int i = 0; i < 8; i++)
        {
            gameState = await _gameStateService.StartRoundAsync(roomCode, i + 1);
            _gameCommunicationService.BroadcastGameEvent(roomCode, GameEngineBroadcastMQ.RoundStartedAction, gameState);
            await Task.Delay(100);

            // ask player for theme
            var players = await _cacheService.GetRoomPlayersAsync(roomCode);
            var selectedTheme = await _gameCommunicationService.AskPlayerThemeAsync(players.ElementAt(i % players.Count), 30);
            
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

            // broadcast assessing answers message to players

            _logger.LogDebug($"[{roomCode}] Round {i + 1} answers collected: {answers.Count}");
            // scoringService
            // broadcast round scores to players
            // await _gameCommunicationService
        }

        // gameStateService.EndGame(roomCode);
        // broadcast end game scores to players
    }
} 
using DrawPT.Common.Interfaces;
using DrawPT.Common.Interfaces.Game;
using DrawPT.Common.Models;
using DrawPT.Common.Models.Game;
using DrawPT.GameEngine.Interfaces;
using DrawPT.GameEngine.Services;
using RabbitMQ.Client;

namespace DrawPT.GameEngine;

public class GameEngine : IGameEngine
{
    private readonly IGameCommunicationService _gameCommunicationService;
    private readonly IModel _channel;
    private readonly ICacheService _cacheService;
    private readonly IThemeService _themeService;
    private readonly IQuestionService _questionService;
    private readonly ILogger<GameEngine> _logger;

    public GameEngine(
        IConnection rabbitMqConnection,
        IThemeService themeService,
        ICacheService cacheService,
        IQuestionService questionService,
        IGameCommunicationService gameCommunicationService,
        ILogger<GameEngine> logger)
    {
        _themeService = themeService;
        _cacheService = cacheService;
        _gameCommunicationService = gameCommunicationService;
        _questionService = questionService;
        _channel = rabbitMqConnection.CreateModel();
        _logger = logger;
    }

    public async Task PlayGameAsync(string roomCode)
    {
        // Broadcast start game message
        // gameStateService.StartGame(roomCode);

        for (int i = 0; i < 8; i++)
        {
            // gameStateService.StartRound(roomCode, i);
            var players = await _cacheService.GetRoomPlayersAsync(roomCode);

            var selectedTheme = await _gameCommunicationService.AskPlayerTheme(players.ElementAt(i%players.Count), 30);
            var question = await _questionService.GenerateQuestionAsync(selectedTheme);
            question.RoundNumber = i + 1;

            // collect answers
            List<Task<PlayerAnswer>> playerAnswers = new(players.Count);
            foreach (var player in players)
            {
                playerAnswers.Add(_gameCommunicationService.AskPlayerQuestion(player, question, 30));
            }

            // empty game check
            if (playerAnswers.Count == 0)
                break;

            await Task.WhenAll(playerAnswers);

            var answers = new List<PlayerAnswer>();
            foreach (var answer in playerAnswers.Select(t => t.Result))
                answers.Add(answer);

            _logger.LogDebug($"[{roomCode}] Round {i + 1} answers collected: {answers.Count}");
            // scoringService

            // broadcast round scores to players
            // clientCommunicationService
        }

        // gameStateService.EndGame(roomCode);
        // broadcast end game scores to players
    }
} 
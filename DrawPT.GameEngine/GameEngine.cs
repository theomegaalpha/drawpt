using DrawPT.Common.Interfaces;
using DrawPT.GameEngine.Interfaces;
using RabbitMQ.Client;

namespace DrawPT.GameEngine;

public class GameEngine : IGameEngine
{
    private readonly IGameCommunicationService _gameCommunicationService;
    private readonly IModel _channel;
    private readonly ICacheService _cacheService;

    public GameEngine(
        ICacheService cacheService,
        IConnection rabbitMqConnection,
        IGameCommunicationService gameCommunicationService)
    {
        _cacheService = cacheService;
        _gameCommunicationService = gameCommunicationService;
        _channel = rabbitMqConnection.CreateModel();
    }

    public async Task PlayGameAsync(string roomCode)
    {
        // Broadcast start game message
        // gameStateService.StartGame(roomCode);

        for (int i = 0; i < 8; i++)
        {
            // gameStateService.StartRound(roomCode, i);
            var players = await _cacheService.GetRoomPlayersAsync(roomCode);

            // get 5 theme options from the database
            // themesService
            //var selectedTheme = await _gameCommunicationService.RequestUserInputAsync("hi", players.First().ConnectionId, 60000);

            // generate question
            // questionService

            // collect answers
            //var answer = await _gameCommunicationService.RequestUserInputAsync("hi", players.First().ConnectionId, 60000);

            // scoringService

            // broadcast round scores to players
            // clientCommunicationService
        }

        // gameStateService.EndGame(roomCode);
        // broadcast end game scores to players
    }
} 
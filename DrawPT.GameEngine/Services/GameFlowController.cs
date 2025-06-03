using DrawPT.Common.Interfaces;
using DrawPT.Common.Models;
using DrawPT.Common.Services;
using DrawPT.GameEngine.Interfaces;
using RabbitMQ.Client;

namespace DrawPT.GameEngine.Services;

public class GameFlowController : IGameFlowController
{
    private readonly IRoundOrchestrator _roundOrchestrator;
    private readonly IModel _channel;
    private readonly ICacheService _cacheService;

    public GameFlowController(
        ICacheService cacheService,
        IConnection rabbitMqConnection,
        IRoundOrchestrator roundOrchestrator)
    {
        _cacheService = cacheService;
        _roundOrchestrator = roundOrchestrator;
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
            var selectedTheme = await _roundOrchestrator.RequestUserInputAsync("hi", players.First().ConnectionId, 60000);

            // generate question
            // questionService

            // collect answers
            var answer = await _roundOrchestrator.RequestUserInputAsync("hi", players.First().ConnectionId, 60000);

            // scoringService

            // broadcast round scores to players
            // clientCommunicationService
        }

        // gameStateService.EndGame(roomCode);
        // broadcast end game scores to players
    }
} 
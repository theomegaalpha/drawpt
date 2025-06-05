using DrawPT.Common.Configuration;
using DrawPT.Common.Interfaces;
using DrawPT.Common.Models.Game;
using DrawPT.GameEngine.Interfaces;

namespace DrawPT.GameEngine.Services
{
    public class GameStateService : IGameStateService
    {
        private readonly IGameCommunicationService _gameCommunicationService;
        private readonly ICacheService _cacheService;

        public GameStateService(ICacheService cacheService,
            IGameCommunicationService gameCommunicationService)
        {
            _cacheService = cacheService;
            _gameCommunicationService = gameCommunicationService;
        }

        public async Task StartGameAsync(string roomCode)
        {
            var gameState = await _cacheService.GetGameState(roomCode);
            gameState ??= new GameState() { RoomCode = roomCode };
            await _cacheService.SetGameState(gameState);

            _gameCommunicationService.BroadcastGameEvent(roomCode, GameEngineBroadcastMQ.GameStartedAction);
        }

        public async Task StartRoundAsync(string roomCode, int roundNumber)
        {
            // Logic to start a round
            await Task.CompletedTask;
        }

        public async Task EndGameAsync(string roomCode)
        {
            var gameState = await _cacheService.GetGameState(roomCode);
            gameState ??= new GameState() { RoomCode = roomCode };
            await _cacheService.SetGameState(gameState);
        }
    }
}

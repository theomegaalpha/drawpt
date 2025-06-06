using DrawPT.Common.Configuration;
using DrawPT.Common.Interfaces;
using DrawPT.Common.Interfaces.Game;
using DrawPT.Common.Models.Game;
using DrawPT.GameEngine.Interfaces;

namespace DrawPT.GameEngine.Services
{
    public class GameStateService : IGameStateService
    {
        private readonly ICacheService _cacheService;

        public GameStateService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<IGameState> StartGameAsync(string roomCode)
        {
            var gameState = await _cacheService.GetGameState(roomCode);
            gameState ??= new GameState() { RoomCode = roomCode };
            await _cacheService.SetGameState(gameState);
            return gameState;
        }

        public async Task<IGameState> StartRoundAsync(string roomCode, int roundNumber)
        {
            var gameState = await _cacheService.GetGameState(roomCode);
            gameState ??= new GameState() { RoomCode = roomCode, CurrentRound = roundNumber };
            await _cacheService.SetGameState(gameState);
            return gameState;
        }

        public async Task<IGameState> EndGameAsync(string roomCode)
        {
            var gameState = await _cacheService.GetGameState(roomCode);
            gameState ??= new GameState() { RoomCode = roomCode };
            await _cacheService.SetGameState(gameState);
            return gameState;
        }
    }
}

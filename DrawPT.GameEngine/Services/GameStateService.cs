using DrawPT.Common.Constants;
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
            gameState ??= new GameState() { RoomCode = roomCode, CurrentStatus = GameStatus.JustStarted };
            await _cacheService.SetGameState(gameState);
            return gameState;
        }

        public async Task<IGameState> StartRoundAsync(string roomCode, int roundNumber)
        {
            var gameState = await _cacheService.GetGameState(roomCode);
            gameState ??= new GameState() { RoomCode = roomCode, CurrentRound = roundNumber, CurrentStatus = GameStatus.StartingRound };
            await _cacheService.SetGameState(gameState);
            return gameState;
        }

        public async Task<IGameState> AskThemeAsync(string roomCode)
        {
            var gameState = await _cacheService.GetGameState(roomCode);
            gameState ??= new GameState() { RoomCode = roomCode, CurrentStatus = GameStatus.AskingTheme };
            await _cacheService.SetGameState(gameState);
            return gameState;
        }

        public async Task<IGameState> ChooseThemeAsync(string roomCode)
        {
            var gameState = await _cacheService.GetGameState(roomCode);
            gameState ??= new GameState() { RoomCode = roomCode };
            await _cacheService.SetGameState(gameState);
            return gameState;
        }

        public async Task<IGameState> AskImagePromptAsync(string roomCode)
        {
            var gameState = await _cacheService.GetGameState(roomCode);
            gameState ??= new GameState() { RoomCode = roomCode, CurrentStatus = GameStatus.AskingImagePrompt };
            await _cacheService.SetGameState(gameState);
            return gameState;
        }

        public async Task<IGameState> AnswerImagePromptAsync(string roomCode)
        {
            var gameState = await _cacheService.GetGameState(roomCode);
            gameState ??= new GameState() { RoomCode = roomCode };
            await _cacheService.SetGameState(gameState);
            return gameState;
        }

        public async Task<IGameState> AskQuestionAsync(string roomCode)
        {
            var gameState = await _cacheService.GetGameState(roomCode);
            gameState ??= new GameState() { RoomCode = roomCode, CurrentStatus = GameStatus.AskingQuestion };
            await _cacheService.SetGameState(gameState);
            return gameState;
        }

        public async Task<IGameState> EndGameAsync(string roomCode)
        {
            var gameState = await _cacheService.GetGameState(roomCode);
            gameState ??= new GameState() { RoomCode = roomCode, CurrentStatus = GameStatus.Completed };
            await _cacheService.SetGameState(gameState);
            return gameState;
        }
    }
}

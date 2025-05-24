namespace DrawPT.Api.Hubs
{
    using DrawPT.Api.Engine;
    using DrawPT.Api.Services;
    using Microsoft.AspNetCore.SignalR;

    public class GameHub : Hub<IGamePlayer>
    {
        private readonly GameCollection _gameCollection;
        private readonly CacheService _cacheService;

        public GameHub(GameCollection gameCollection, CacheService cacheService)
        {
            _gameCollection = gameCollection;
            _cacheService = cacheService;
        }

        public async Task<bool> JoinGame(string roomCode, Guid playerId)
        {
            var player = await _cacheService.GetPlayerAsync(playerId);
            if (player == null)
                return false;
            return _gameCollection.AddPlayerToGameAsync(Context, roomCode, player).Result;
        }

        public bool StartGame()
        {
            var game = _gameCollection.GetGameByConnectionContext(Context);
            if (game == null)
                return false;

            _ = Task.Run(game.PlayGame);
            return true;
        }
    }
}
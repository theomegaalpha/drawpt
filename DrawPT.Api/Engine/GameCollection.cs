//using DrawPT.Data.Models;
//using DrawPT.Api.Services;
//using Microsoft.AspNetCore.SignalR;
//using System.Collections.Concurrent;

//namespace DrawPT.Api.Engine
//{
//    public class GameCollection
//    {
//        private readonly CacheService _cacheService;
//        private readonly IGameFactory _gameFactory;
//        // TODO: pull this out cacheService to be scalable
//        public readonly ConcurrentDictionary<string, Game> _activeGames = new();
//        // The key into the per connection dictionary used to look up the current game;
//        private static readonly object _gameKey = new();

//        public GameCollection(IGameFactory gameFactory, CacheService cacheService)
//        {
//            _gameFactory = gameFactory;
//            _cacheService = cacheService;
//        }

//        public async Task<bool> AddPlayerToGameAsync(HubCallerContext hubCallerContext, string roomCode, Player player)
//        {
//            // There's already a game associated with this player, just return it
//            if (hubCallerContext.Items[_gameKey] is Game g)
//                return true;

//            if (!_activeGames.TryGetValue(roomCode, out var game))
//            {
//                game = _gameFactory.CreateGame(roomCode);
//                if (_activeGames.TryAdd(roomCode, game))
//                {
//                    // Remove the game when it completes
//                    game.Completed.UnsafeRegister(_ =>
//                    {
//                        _cacheService.CloseRoom(roomCode);
//                        _activeGames.TryRemove(roomCode, out var _);
//                    },
//                    null);
//                }
//            }

//            player.ConnectionId = hubCallerContext.ConnectionId;
//            await _cacheService.UpdatePlayerAsync(player);

//            // Try to add the player to this game. It'll return false if the game is full.
//            if (!await game.AddPlayerAsync(hubCallerContext.ConnectionId, player))
//                return false;

//            // Store the association of this player to this game
//            hubCallerContext.Items[_gameKey] = game;

//            // When the player disconnects, remove them from the game
//            hubCallerContext.ConnectionAborted.Register(() =>
//            {
//                // We can't wait here (since this is synchronous), so fire and forget
//                _ = game.RemovePlayerAsync(hubCallerContext.ConnectionId, player);
//            });

//            // When the game ends, remove the game from the player (they can join another game)
//            game.Completed.Register(() =>
//            {
//                hubCallerContext.Items.Remove(_gameKey);
//            });

//            return true;
//        }

//        public Game? GetGameByConnectionContext(HubCallerContext hubCallerContext)
//        {
//            if (hubCallerContext.Items[_gameKey] is Game g)
//            {
//                return g;
//            }
//            return null;
//        }
//    }
//}

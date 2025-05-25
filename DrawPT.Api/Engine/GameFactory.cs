using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace DrawPT.Api.Engine
{
    public interface IGameFactory
    {
        Game CreateGame(string roomCode);
    }

    public class GameFactory : IGameFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public GameFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Game CreateGame(string roomCode)
        {
            using var scope = _serviceProvider.CreateScope();
            var game = scope.ServiceProvider.GetRequiredService<Game>();
            game.RoomCode = roomCode;
            game.SetGroupName();
            return game;
        }
    }
} 
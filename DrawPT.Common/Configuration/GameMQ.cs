namespace DrawPT.Common.Configuration
{
    public class GameMQ
    {
        public const string ExchangeName = "game";
        public const string QueueName = "game";
        public const string RoutingKey = "game.#";

        public const string GameStart = "game_started";

        public static class RoutingKeys
        {
            public static string GameStarted(string roomCode) => $"game.{roomCode}.{GameStart}";
        }
    }
} 
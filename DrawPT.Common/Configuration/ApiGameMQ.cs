namespace DrawPT.Common.Configuration
{
    public class ApiGameMQ
    {
        public const string ExchangeName = "api";
        public const string QueueName = "api";
        public const string RoutingKey = "api.#";

        public const string GameStart = "game_started";

        public static class RoutingKeys
        {
            public static string GameStarted(string roomCode) => $"api.{roomCode}.{GameStart}";
        }
    }
} 
namespace DrawPT.Common.Configuration
{
    public class GameRequestMQ
    {
        public const string ExchangeName = "game_request";
        public const string QueueName = "game_request";
        public const string RoutingKey = "game_request.#";

        public const string GameStarted = "game_started";

        public static class RoutingKeys
        {
            public static string GameStarted(string roomCode) => $"game_request.{roomCode}.{GameStarted}";
        }
    }
} 
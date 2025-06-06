namespace DrawPT.Common.Configuration
{
    public class ApiMasterMQ
    {
        public const string ExchangeName = "api";
        public const string QueueName = "api";
        public const string RoutingKey = "api.#";

        public const string GameStartedAction = "game_started";

        public static class RoutingKeys
        {
            public static string GameStarted(string roomCode) => $"api.{roomCode}.{GameStartedAction}";
        }
    }
} 
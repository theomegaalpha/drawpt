namespace DrawPT.Common.Configuration
{
    public class GameRequestMQ
    {
        public const string ExchangeName = "game_request";
        public const string QueueName = "game_request";
        public const string RoutingKey = "game_request.#";

        public const string Theme = "ask_theme";
        public const string Question = "ask_question";

        public static class RoutingKeys
        {
            public static string AskTheme(string connectionId) => $"game_request.{connectionId}.{Theme}";
            public static string AskQuestion(string connectionId) => $"game_request.{connectionId}.{Question}";
        }
    }
} 
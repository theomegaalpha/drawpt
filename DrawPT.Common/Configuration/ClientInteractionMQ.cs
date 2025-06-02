namespace DrawPT.Common.Configuration
{
    public class ClientInteractionMQ
    {
        public const string ExchangeName = "client_interact";
        public const string QueueName = "client_interact";
        public const string RoutingKey = "client_interact.#";

        public const string AskTheme = "ask_theme";
        public const string AskQuestion = "ask_question";

        public static class RoutingKeys
        {
            public static string AskTheme(string connectionId) => $"client_interact.{connectionId}.{AskTheme}";
            public static string AskQuestion(string connectionId) => $"client_interact.{connectionId}.{AskQuestion}";
        }
    }
} 
namespace DrawPT.Common.Configuration
{
    public class GameEngineRequestMQ
    {
        public const string ExchangeName = "game_engine";
        public static string QueueName(string connectionId) => $"game_engine.{connectionId}";
        public static string RoutingKey(string connectionId) => $"game_engine.{connectionId}.#";

        public const string Theme = "ask_theme";
        public const string Question = "ask_question";

        public static class RoutingKeys
        {
            public static string AskTheme(string connectionId) => $"game_engine.{connectionId}.{Theme}";
            public static string AskQuestion(string connectionId) => $"game_engine.{connectionId}.{Question}";
        }
    }
} 
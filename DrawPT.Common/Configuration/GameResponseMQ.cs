namespace DrawPT.Common.Configuration
{
    public class GameResponseMQ
    {
        public const string ExchangeName = "game_response";
        public const string QueueName = "game_response";
        public const string RoutingKey = "game_response.#";

        public const string GameStarted = "game_started";

        public static class RoutingKeys
        {
            public const string AnswerTheme = "game_response.answer_theme";
            public const string AnswerQuestion = "game_response.answer_question";
        }
    }
} 
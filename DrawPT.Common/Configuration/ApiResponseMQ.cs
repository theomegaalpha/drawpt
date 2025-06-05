namespace DrawPT.Common.Configuration
{
    public class ApiResponseMQ
    {
        public const string ExchangeName = "api";
        public static string QueueName(string connectionId) => $"api.{connectionId}";
        public static string RoutingKey(string connectionId) => $"api.{connectionId}.#";


        public static class RoutingKeys
        {
            public static string AnswerTheme(string connectionId) => $"api.{connectionId}.answer_theme";
            public static string AnswerQuestion(string connectionId) => $"api.{connectionId}.answer_question";
        }
    }
} 
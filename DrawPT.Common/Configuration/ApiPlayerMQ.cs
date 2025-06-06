namespace DrawPT.Common.Configuration
{
    public class ApiPlayerMQ
    {
        public const string ExchangeName = "api.player";
        public static string QueueName(string roomCode) => $"api.player.{roomCode}";
        public static string RoutingKey(string roomCode) => $"api.player.{roomCode}.#";

        public const string PlayerLeftAction = "player_left";

        public static class RoutingKeys
        {
            public static string PlayerLeft(string roomCode) => $"api.player.{roomCode}.{PlayerLeftAction}";
        }
    }
} 
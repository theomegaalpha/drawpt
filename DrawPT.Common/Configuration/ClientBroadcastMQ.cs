namespace DrawPT.Common.Configuration
{
    public class ClientBroadcastMQ
    {
        public const string ExchangeName = "client_broadcast";
        public const string QueueName = "client_broadcast";
        public const string RoutingKey = "client_broadcast.#";

        public const string PlayerJoinedAction = "player_joined";
        public const string PlayerLeftAction = "player_left";

        public const string GameStarted = "game_started";

        public static class RoutingKeys
        {
            public static string PlayerScore(string roomCode) => $"client_broadcast.{roomCode}.player.score";
            public static string GameStarted(string roomCode) => $"client_broadcast.{roomCode}.{GameStarted}";
            public static string GameEnded(string roomCode) => $"client_broadcast.{roomCode}.game.ended";
            public static string RoundStarted(string roomCode) => $"client_broadcast.{roomCode}.round.started";
            public static string RoundEnded(string roomCode) => $"client_broadcast.{roomCode}.round.ended";
            public static string PlayerJoined(string roomCode) => $"client_broadcast.{roomCode}.{PlayerJoinedAction}";
            public static string PlayerLeft(string roomCode) => $"client_broadcast.{roomCode}.{PlayerLeftAction}";

            public static class Matchmaking
            {
                public const string RoomCreated = "room.created";
            }
        }
    }
} 
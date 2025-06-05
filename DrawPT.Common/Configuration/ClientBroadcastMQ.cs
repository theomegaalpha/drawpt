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
        public const string RoundStarted = "round_started";
        public const string AssessingAnswers = "assessing_answers";
        public const string PlayerScoreUpdate = "player_score_update";
        public const string RoundEnded = "round_ended";
        public const string GameEnded = "game_ended";

        public const string WriteMessage = "write_message";

        public static class RoutingKeys
        {
            public static string GameStarted(string roomCode) => $"client_broadcast.{roomCode}.{ClientBroadcastMQ.GameStarted}";
            public static string RoundStarted(string roomCode) => $"client_broadcast.{roomCode}.{ClientBroadcastMQ.RoundStarted}";
            public static string AssessingAnswers(string roomCode) => $"client_broadcast.{roomCode}.{ClientBroadcastMQ.AssessingAnswers}";
            public static string RoundEnded(string roomCode) => $"client_broadcast.{roomCode}.{ClientBroadcastMQ.RoundEnded}";
            public static string GameEnded(string roomCode) => $"client_broadcast.{roomCode}.{ClientBroadcastMQ.GameEnded}";

            public static string PlayerScoreUpdate(string roomCode) => $"client_broadcast.{roomCode}.{ClientBroadcastMQ.PlayerScoreUpdate}";
            public static string WriteMessage(string roomCode) => $"client_broadcast.{roomCode}.{ClientBroadcastMQ.WriteMessage}";

            public static string PlayerJoined(string roomCode) => $"client_broadcast.{roomCode}.{PlayerJoinedAction}";
            public static string PlayerLeft(string roomCode) => $"client_broadcast.{roomCode}.{PlayerLeftAction}";

            public static class Matchmaking
            {
                public const string RoomCreated = "room.created";
            }
        }
    }
} 
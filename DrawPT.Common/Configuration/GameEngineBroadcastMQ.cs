namespace DrawPT.Common.Configuration
{
    public class GameEngineBroadcastMQ
    {
        public const string ExchangeName = "game_engine";
        public static string QueueName(string roomCode) => $"game_engine.{roomCode}";
        public static string RoutingKey(string roomCode) => $"game_engine.{roomCode}.#";

        public const string PlayerJoinedAction = "player_joined";
        public const string PlayerLeftAction = "player_left";

        public const string GameStartedAction = "game_started";
        public const string RoundStartedAction = "round_started";
        public const string AssessingAnswersAction = "assessing_answers";
        public const string PlayerScoreUpdateAction = "player_score_update";
        public const string RoundEndedAction = "round_ended";
        public const string GameEndedAction = "game_ended";

        public const string WriteMessageAction = "write_message";

        public static class RoutingKeys
        {
            public static string GameStarted(string roomCode) => $"game_engine.{roomCode}.{GameStartedAction}";
            public static string RoundStarted(string roomCode) => $"game_engine.{roomCode}.{RoundStartedAction}";
            public static string AssessingAnswers(string roomCode) => $"game_engine.{roomCode}.{AssessingAnswersAction}";
            public static string RoundEnded(string roomCode) => $"game_engine.{roomCode}.{RoundEndedAction}";
            public static string GameEnded(string roomCode) => $"game_engine.{roomCode}.{GameEndedAction}";

            public static string PlayerScoreUpdate(string roomCode) => $"game_engine.{roomCode}.{PlayerScoreUpdateAction}";
            public static string WriteMessage(string roomCode) => $"game_engine.{roomCode}.{WriteMessageAction}";

            public static string PlayerJoined(string roomCode) => $"game_engine.{roomCode}.{PlayerJoinedAction}";
            public static string PlayerLeft(string roomCode) => $"game_engine.{roomCode}.{PlayerLeftAction}";
        }
    }
} 
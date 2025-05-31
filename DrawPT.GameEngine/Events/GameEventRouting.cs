namespace DrawPT.GameEngine.Events
{
    /// <summary>
    /// Provides routing key management for game events
    /// </summary>
    public static class GameEventRouting
    {
        private const string ExchangeName = "game_events";
        private const string GamePrefix = "game";
        private const string PlayerPrefix = "player";
        private const string RoundPrefix = "round";
        private const string QuestionPrefix = "question";

        /// <summary>
        /// Gets the exchange name for game events
        /// </summary>
        public static string Exchange => ExchangeName;

        /// <summary>
        /// Creates a routing key for a game-specific event
        /// </summary>
        public static string CreateGameRoutingKey(string gameId, GameEventType eventType) =>
            $"{GamePrefix}.{gameId}.{eventType.ToString().ToLower()}";

        /// <summary>
        /// Creates a routing key for a player-specific event
        /// </summary>
        public static string CreatePlayerRoutingKey(string gameId, string playerId, GameEventType eventType) =>
            $"{GamePrefix}.{gameId}.{PlayerPrefix}.{playerId}.{eventType.ToString().ToLower()}";

        /// <summary>
        /// Creates a routing key for a round-specific event
        /// </summary>
        public static string CreateRoundRoutingKey(string gameId, int roundNumber, GameEventType eventType) =>
            $"{GamePrefix}.{gameId}.{RoundPrefix}.{roundNumber}.{eventType.ToString().ToLower()}";

        /// <summary>
        /// Creates a routing key for a question-specific event
        /// </summary>
        public static string CreateQuestionRoutingKey(string gameId, string questionId, GameEventType eventType) =>
            $"{GamePrefix}.{gameId}.{QuestionPrefix}.{questionId}.{eventType.ToString().ToLower()}";

        /// <summary>
        /// Creates a queue name for a game
        /// </summary>
        public static string CreateGameQueueName(string gameId) =>
            $"game_events_{gameId}";

        /// <summary>
        /// Creates a queue name for a player
        /// </summary>
        public static string CreatePlayerQueueName(string gameId, string playerId) =>
            $"player_events_{gameId}_{playerId}";

        /// <summary>
        /// Creates a queue name for a round
        /// </summary>
        public static string CreateRoundQueueName(string gameId, int roundNumber) =>
            $"round_events_{gameId}_{roundNumber}";

        /// <summary>
        /// Creates a queue name for a question
        /// </summary>
        public static string CreateQuestionQueueName(string gameId, string questionId) =>
            $"question_events_{gameId}_{questionId}";

        /// <summary>
        /// Creates a binding pattern for all game events
        /// </summary>
        public static string CreateGameBindingPattern(string gameId) =>
            $"{GamePrefix}.{gameId}.*";

        /// <summary>
        /// Creates a binding pattern for all player events
        /// </summary>
        public static string CreatePlayerBindingPattern(string gameId) =>
            $"{GamePrefix}.{gameId}.{PlayerPrefix}.*";

        /// <summary>
        /// Creates a binding pattern for all round events
        /// </summary>
        public static string CreateRoundBindingPattern(string gameId) =>
            $"{GamePrefix}.{gameId}.{RoundPrefix}.*";

        /// <summary>
        /// Creates a binding pattern for all question events
        /// </summary>
        public static string CreateQuestionBindingPattern(string gameId) =>
            $"{GamePrefix}.{gameId}.{QuestionPrefix}.*";

        /// <summary>
        /// Gets the event type from a routing key
        /// </summary>
        public static GameEventType GetEventTypeFromRoutingKey(string routingKey)
        {
            var eventTypeStr = routingKey.Split('.').Last();
            return Enum.Parse<GameEventType>(eventTypeStr, true);
        }
    }
} 
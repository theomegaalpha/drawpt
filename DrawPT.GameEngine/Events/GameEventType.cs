namespace DrawPT.GameEngine.Events
{
    /// <summary>
    /// Represents the types of game events that can be published
    /// </summary>
    public enum GameEventType
    {
        // Player events
        PlayerJoined,
        PlayerLeft,
        PlayerScoreUpdated,

        // Round events
        RoundStarted,
        RoundEnded,
        AnswerSubmitted,

        // Question events
        ThemeSelected,
        QuestionGenerated,

        // Game events
        GameStarted,
        GameEnded
    }
} 
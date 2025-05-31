using DrawPT.GameEngine.Models;

namespace DrawPT.GameEngine.Events
{
    /// <summary>
    /// Event raised when a game starts
    /// </summary>
    public class GameStartedEvent : BaseGameEvent
    {
        public string GameId { get; set; } = string.Empty;
        public override GameEventType EventType => GameEventType.GameStarted;
    }

    /// <summary>
    /// Event raised when a game ends
    /// </summary>
    public class GameEndedEvent : BaseGameEvent
    {
        public string GameId { get; set; } = string.Empty;
        public override GameEventType EventType => GameEventType.GameEnded;
    }

    /// <summary>
    /// Event raised when a player joins a game
    /// </summary>
    public class PlayerJoinedEvent : BaseGameEvent
    {
        public string GameId { get; set; } = string.Empty;
        public string PlayerId { get; set; } = string.Empty;
        public override GameEventType EventType => GameEventType.PlayerJoined;
    }

    /// <summary>
    /// Event raised when a player leaves a game
    /// </summary>
    public class PlayerLeftEvent : BaseGameEvent
    {
        public string GameId { get; set; } = string.Empty;
        public string PlayerId { get; set; } = string.Empty;
        public override GameEventType EventType => GameEventType.PlayerLeft;
    }

    /// <summary>
    /// Event raised when a player's score is updated
    /// </summary>
    public class PlayerScoreUpdatedEvent : BaseGameEvent
    {
        public string GameId { get; set; } = string.Empty;
        public string PlayerId { get; set; } = string.Empty;
        public int NewScore { get; set; }
        public override GameEventType EventType => GameEventType.PlayerScoreUpdated;
    }

    /// <summary>
    /// Event raised when a round starts
    /// </summary>
    public class RoundStartedEvent : BaseGameEvent
    {
        public string GameId { get; set; } = string.Empty;
        public int RoundNumber { get; set; }
        public override GameEventType EventType => GameEventType.RoundStarted;
    }

    /// <summary>
    /// Event raised when a round ends
    /// </summary>
    public class RoundEndedEvent : BaseGameEvent
    {
        public string GameId { get; set; } = string.Empty;
        public int RoundNumber { get; set; }
        public override GameEventType EventType => GameEventType.RoundEnded;
    }

    /// <summary>
    /// Event raised when an answer is submitted
    /// </summary>
    public class AnswerSubmittedEvent : BaseGameEvent
    {
        public string GameId { get; set; } = string.Empty;
        public int RoundNumber { get; set; }
        public string PlayerId { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public override GameEventType EventType => GameEventType.AnswerSubmitted;
    }

    /// <summary>
    /// Event raised when a theme is selected
    /// </summary>
    public class ThemeSelectedEvent : BaseGameEvent
    {
        public string GameId { get; set; } = string.Empty;
        public string QuestionId { get; set; } = string.Empty;
        public string Theme { get; set; } = string.Empty;
        public override GameEventType EventType => GameEventType.ThemeSelected;
    }

    /// <summary>
    /// Event raised when a question is generated
    /// </summary>
    public class QuestionGeneratedEvent : BaseGameEvent
    {
        public string GameId { get; set; } = string.Empty;
        public string QuestionId { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public override GameEventType EventType => GameEventType.QuestionGenerated;
    }
} 
using DrawPT.GameEngine.Models;

namespace DrawPT.GameEngine.Events
{
    /// <summary>
    /// Event raised when a game starts
    /// </summary>
    public class GameStartedEvent : BaseGameEvent
    {
        public override string EventType => "GameStarted";
        public GameConfiguration Configuration { get; set; } = null!;
    }

    /// <summary>
    /// Event raised when a game ends
    /// </summary>
    public class GameEndedEvent : BaseGameEvent
    {
        public override string EventType => "GameEnded";
        public GameResults Results { get; set; } = null!;
    }

    /// <summary>
    /// Event raised when a player joins the game
    /// </summary>
    public class PlayerJoinedEvent : BaseGameEvent
    {
        public override string EventType => "PlayerJoined";
        public Player Player { get; set; } = null!;
    }

    /// <summary>
    /// Event raised when a player leaves the game
    /// </summary>
    public class PlayerLeftEvent : BaseGameEvent
    {
        public override string EventType => "PlayerLeft";
        public Player Player { get; set; } = null!;
    }

    /// <summary>
    /// Event raised when a round starts
    /// </summary>
    public class RoundStartedEvent : BaseGameEvent
    {
        public override string EventType => "RoundStarted";
        public GameRound Round { get; set; } = null!;
    }

    /// <summary>
    /// Event raised when a round ends
    /// </summary>
    public class RoundEndedEvent : BaseGameEvent
    {
        public override string EventType => "RoundEnded";
        public GameRound Round { get; set; } = null!;
    }

    /// <summary>
    /// Event raised when a player submits an answer
    /// </summary>
    public class AnswerSubmittedEvent : BaseGameEvent
    {
        public override string EventType => "AnswerSubmitted";
        public GameAnswer Answer { get; set; } = null!;
    }
} 
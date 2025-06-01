using DrawPT.GameEngine.Models;

namespace DrawPT.GameEngine.Interfaces;

public interface IGameCommunicationService
{
    Task BroadcastGameEventAsync(GameEvent gameEvent);
    Task SendToPlayerAsync(string connectionId, PlayerEvent playerEvent);
    Task BroadcastRoundResultsAsync(GameRound round);
    Task BroadcastFinalResultsAsync(GameResults results);
}

public abstract class GameEvent
{
    public required string RoomCode { get; init; }
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}

public class GameStartedEvent : GameEvent
{
    public required GameConfiguration Configuration { get; init; }
}

public class GameCompletedEvent : GameEvent
{
    public required GameResults Results { get; init; }
}

public abstract class PlayerEvent
{
    public required string ConnectionId { get; init; }
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}

public class PlayerJoinedEvent : PlayerEvent
{
    public required Player Player { get; init; }
}

public class PlayerLeftEvent : PlayerEvent
{
    public required Player Player { get; init; }
}

public class ScoreUpdatedEvent : PlayerEvent
{
    public required int NewScore { get; init; }
    public required int BonusPoints { get; init; }
} 
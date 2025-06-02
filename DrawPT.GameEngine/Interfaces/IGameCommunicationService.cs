using DrawPT.Common.Models;

namespace DrawPT.GameEngine.Interfaces;

public interface IGameCommunicationService
{
    Task BroadcastGameEventAsync(GameEvent gameEvent);
    Task SendToPlayerAsync(string connectionId, PlayerEvent playerEvent);
    Task BroadcastRoundResultsAsync(GameRound round);
    Task BroadcastFinalResultsAsync(GameResults results);
    Task<string> AskPlayerForThemeAsync(string connectionId, List<string> themes, TimeSpan timeout);
    Task<PlayerAnswer> AskPlayerForAnswerAsync(string connectionId, string imageUrl, TimeSpan timeout);
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

public class ThemeSelectionStartedEvent : GameEvent
{
    public required string SelectorId { get; init; }
}

public class ThemeSelectedEvent : GameEvent
{
    public required string ThemeName { get; init; }
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
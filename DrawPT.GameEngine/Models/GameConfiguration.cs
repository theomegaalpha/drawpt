namespace DrawPT.GameEngine.Models;

public class GameConfiguration
{
    public int MaxPlayersPerGame { get; init; }
    public int QuestionsPerGame { get; init; }
    public int TimePerQuestion { get; init; }
    public int TimePerThemeSelection { get; init; }
    public int TransitionDelay { get; init; }
} 
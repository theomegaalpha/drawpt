namespace DrawPT.Api.Models
{
    public class GameOptions
    {
        public int MaxPlayersPerGame { get; init; } = 8;
        public int TimePerThemeSelection { get; init; } = 20;
        public int TimePerQuestion { get; init; } = 40;
        public int TransitionDelay { get; init; } = 20;
        public int QuestionsPerGame { get; init; } = 7;
    }
}

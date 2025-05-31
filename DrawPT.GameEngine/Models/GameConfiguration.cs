namespace DrawPT.GameEngine.Models
{
    /// <summary>
    /// Configuration settings for a game instance
    /// </summary>
    public class GameConfiguration
    {
        /// <summary>
        /// Maximum number of players allowed in the game
        /// </summary>
        public int MaxPlayers { get; set; }

        /// <summary>
        /// Number of questions per game
        /// </summary>
        public int QuestionsPerGame { get; set; }

        /// <summary>
        /// Time limit for each question in seconds
        /// </summary>
        public int TimePerQuestion { get; set; }

        /// <summary>
        /// Time limit for theme selection in seconds
        /// </summary>
        public int TimePerThemeSelection { get; set; }

        /// <summary>
        /// Delay between rounds in seconds
        /// </summary>
        public int TransitionDelay { get; set; }

        /// <summary>
        /// Whether gambling is enabled in this game
        /// </summary>
        public bool GamblingEnabled { get; set; }
    }
}
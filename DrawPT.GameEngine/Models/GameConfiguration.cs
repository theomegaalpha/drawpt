namespace DrawPT.GameEngine.Models
{
    /// <summary>
    /// Configuration settings for a game
    /// </summary>
    public class GameConfiguration
    {
        /// <summary>
        /// Gets or sets the maximum number of players allowed in the game
        /// </summary>
        public int MaxPlayers { get; set; }

        /// <summary>
        /// Gets or sets the number of questions per game
        /// </summary>
        public int NumberOfQuestions { get; set; }

        /// <summary>
        /// Gets or sets the time limit for each question in seconds
        /// </summary>
        public int QuestionTimeout { get; set; }

        /// <summary>
        /// Gets or sets the time limit for theme selection in seconds
        /// </summary>
        public int ThemeTimeout { get; set; }

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
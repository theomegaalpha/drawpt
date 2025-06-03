namespace DrawPT.Common.Interfaces.Game
{
    /// <summary>
    /// Configuration settings for a game
    /// </summary>
    public interface IGameConfiguration
    {
        /// <summary>
        /// Gets or sets the maximum number of players allowed in the game
        /// </summary>
        int MaxPlayers { get; set; }

        /// <summary>
        /// Gets or sets the number of questions per game
        /// </summary>
        int NumberOfQuestions { get; set; }

        /// <summary>
        /// Gets or sets the time limit for each question in seconds
        /// </summary>
        int QuestionTimeout { get; set; }

        /// <summary>
        /// Gets or sets the time limit for theme selection in seconds
        /// </summary>
        int ThemeTimeout { get; set; }

        /// <summary>
        /// Delay between rounds in seconds
        /// </summary>
        int TransitionDelay { get; set; }

        /// <summary>
        /// Whether gambling is enabled in this game
        /// </summary>
        bool GamblingEnabled { get; set; }
    }
}
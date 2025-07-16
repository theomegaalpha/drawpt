using DrawPT.Common.Interfaces.Game;

namespace DrawPT.Common.Models.Game
{
    /// <summary>
    /// Configuration settings for a game
    /// </summary>
    public class GameConfiguration : IGameConfiguration
    {
        /// <summary>
        /// Gets or sets the maximum number of players allowed in the game
        /// </summary>
        public int MaxPlayers { get; set; } = 8;

        /// <summary>
        /// Gets or sets the number of questions per game
        /// </summary>
        public int TotalRounds { get; set; } = 6;

        /// <summary>
        /// Gets or sets the time limit for each question in seconds
        /// </summary>
        public int QuestionTimeout { get; set; } = 40;

        /// <summary>
        /// Gets or sets the time limit for theme selection in seconds
        /// </summary>
        public int ThemeTimeout { get; set; } = 30;

        /// <summary>
        /// Delay between rounds in seconds
        /// </summary>
        public int TransitionDelay { get; set; } = 50;

        /// <summary>
        /// Whether gambling is enabled in this game
        /// </summary>
        public bool GamblingEnabled { get; set; } = false;
    }
}

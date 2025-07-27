using DrawPT.Common.Models.Game;
using System.Text.Json.Serialization;

namespace DrawPT.Common.Interfaces.Game
{
    /// <summary>
    /// Configuration settings for a game
    /// </summary>
    [JsonDerivedType(typeof(GameConfiguration), typeDiscriminator: "GameConfiguration")]
    public interface IGameConfiguration
    {
        /// <summary>
        /// Gets or sets the maximum number of players allowed in the game
        /// </summary>
        int MaxPlayers { get; set; }

        /// <summary>
        /// Gets or sets the number of questions per game
        /// </summary>
        int TotalRounds { get; set; }

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
        /// Whether player prompt mode or AI prompt mode is enabled in this game
        /// </summary>
        bool PlayerPromptMode { get; set; }
    }
}

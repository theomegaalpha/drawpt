using Microsoft.EntityFrameworkCore.Migrations;
using OpenTelemetry;

#nullable disable

namespace DrawPT.MigrationService.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdjectives : Migration
    {
        private readonly string[] _adjectives =
        [
            "happy", "sad", "angry", "excited", "bored",
            "hungry", "thirsty", "sleepy", "energetic", "lazy",
            "brave", "cowardly", "curious", "shy", "confident",
            "nervous", "proud", "ashamed", "grumpy", "cheerful",
            "friendly", "unfriendly", "polite", "rude", "kind",
            "mean", "generous", "selfish", "honest", "dishonest",
            "loyal", "disloyal", "optimistic", "pessimistic", "creative",
            "unimaginative", "adventurous", "cautious", "ambitious",
            "patient", "impatient", "calm", "anxious", "organized",
            "disorganized", "neat", "messy", "tidy", "untidy",
            "hardworking", "intelligent", "unintelligent", "wise",
            "strong", "weak", "fast", "slow", "tall", "unambitious",
            "short", "big", "small", "young", "old", "foolish",
            "rich", "poor", "healthy", "sick", "clean",
            "dirty", "beautiful", "ugly", "handsome", "plain",
            "funny", "serious", "silly", "sensible", "noisy",
            "quiet", "loud", "bright", "dull", "colorful",
            "colorless", "warm", "cold", "hot", "cool",
            "wet", "dry", "smooth", "rough", "hard",
            "soft", "heavy", "light", "thick", "thin",
            "wide", "narrow", "quick"
        ];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ensure schema exists
            migrationBuilder.EnsureSchema(name: "ref");
            migrationBuilder.InsertData(
                schema: "ref",
                table: "Adjectives",
                column: "Adjective",
                values: _adjectives);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                schema: "ref",
                name: "Adjectives");
        }
    }
} 
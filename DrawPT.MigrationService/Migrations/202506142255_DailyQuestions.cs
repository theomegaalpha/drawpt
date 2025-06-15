using Microsoft.EntityFrameworkCore.Migrations;

namespace DrawPT.MigrationService.Migrations
{
    /// <inheritdoc />
    public partial class DailyQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "game");

            migrationBuilder.CreateTable(
                name: "DailyQuestions",
                schema: "game",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Style = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    Theme = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    OriginalPrompt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyThemes",
                schema: "game",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Style = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    Theme = table.Column<string>(type: "nvarchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyThemes", x => x.Id);
                });

            string[] _themes = [
                "anime:Legendary swordsman",
                "anime:Giant mecha",
                "anime:Post-apocalyptic",
                "anime:Bounty hunter",
                "anime:Superpowered duel",
                "anime:Idol singer",
                "anime:Demon-slaying",
                "anime:Ethereal spirit",
                "anime:Cyber-enhanced ninja",
                "anime:Yokai",

                "cyberpunk:Katana-wielding mercenary",
                "cyberpunk:Augmented hacker",
                "cyberpunk:Street racer",
                "cyberpunk:Glitching AI",
                "cyberpunk:Bounty hunter",
                "cyberpunk:Futuristic night market",
                "cyberpunk:Bio-engineered creature",
                "cyberpunk:Massive vertical metropolis",
                "cyberpunk:Samurai",
                "cyberpunk:Digital dreamscape",

                "painting:Hyper-stylized warrior",
                "painting:Dark fantasy duel",
                "painting:Surreal battlefield",
                "painting:Futuristic gods",
                "painting:Post-apocalyptic",
                "painting:Rogue magician",
                "painting:Masked anti-hero",
                "painting:Anime shonen protagonist",
                "painting:Ancient spirits",
                "painting:Heroic last stand",

                "fantasy:Dragons and airships",
                "fantasy:Celestial knights",
                "fantasy:Dark sorcerer",
                "fantasy:Demon hunter",
                "fantasy:Underground city",
                "fantasy:Cosmic guardian",
                "fantasy:Ancient cyber-magic",
                "fantasy:Doomed hero",
                "fantasy:Futuristic coliseum",
                "fantasy:Otherworldly portal",

                "watercolor:Ethereal warrior",
                "watercolor:Anime-inspired",
                "watercolor:Celestial dragon",
                "watercolor:High-speed chase",
                "watercolor:Cyberpunk",
                "watercolor:Video game hero",

                "digital-art:Neon-slashed samurai",
                "digital-art:Superpowered warrior",
                "digital-art:Glitching reality",
                "digital-art:Legendary warlock",
                "digital-art:VR gladiator",
                "digital-art:Mechanical deity",
                "digital-art:Time distortion",
                "digital-art:Sci-fi concept art",
                "digital-art:Epic fantasy battle",
                "digital-art:Surreal cities"
                ];

            // Convert the themes array to a 2D object array for InsertData
            var themeValues = new object[_themes.Length, 3];
            for (int i = 0; i < _themes.Length; i++)
            {
                var parts = _themes[i].Split(':');
                themeValues[i, 0] = Guid.NewGuid();
                themeValues[i, 1] = parts[0];
                themeValues[i, 2] = parts[1];
            }

            migrationBuilder.InsertData(
                schema: "game",
                table: "DailyThemes",
                columns: ["Id", "Style", "Theme"],
                values: themeValues);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyQuestions",
                schema: "game");

            migrationBuilder.DropTable(
                name: "DailyThemes",
                schema: "game");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace DrawPT.MigrationService.Migrations
{
    /// <inheritdoc />
    public partial class ReSeedThemes : Migration
    {
        private readonly string[] _themes =
        [
            "Legendary swordsman","Giant mecha","Post-apocalyptic","Bounty hunter",
            "Superpowered duel","Idol singer","Demon-slaying","Ethereal spirit",
            "Cyber-enhanced ninja","Yokai","Katana-wielding mercenary","Augmented hacker",
            "Street racer","Glitching AI","Bounty hunter","Futuristic night market",
            "Bio-engineered creature","Massive vertical metropolis","Samurai",
            "Digital dreamscape","Hyper-stylized warrior","Dark fantasy duel",
            "Surreal battlefield","Futuristic gods","Post-apocalyptic",
            "Rogue magician","Masked anti-hero","Anime shonen protagonist",
            "Ancient spirits","Heroic last stand","Dragons and airships",
            "Celestial knights","Dark sorcerer","Demon hunter","Underground city",
            "Cosmic guardian","Ancient cyber-magic","Doomed hero","Futuristic coliseum",
            "Otherworldly portal","Ethereal warrior","Anime-inspired","Celestial dragon",
            "High-speed chase","Cyberpunk","Video game hero","Neon-slashed samurai",
            "Superpowered warrior","Glitching reality","Legendary warlock","VR gladiator",
            "Mechanical deity","Time distortion","Sci-fi concept art","Epic fantasy battle",
            "Surreal cities","Cyberpunk dystopia","Fantasy landscapes","Anime-inspired fantasy",
            "Ethereal warrior","Anime-inspired","Celestial dragon", "Legendary warlock",
            "Miniaturecore", "Academia","Under the Sea","Horrorifying Creature","Science Fantasy",
            "Goblins","Supernatural","Fantasy","Sci-Fi","Aliens & UFOs","Mystical",
            "Lucid Scifi"
        ];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "ref",
                table: "Themes",
                keyColumn: "Name",
                keyValues: _themes);

            var values = new object[_themes.Length, 2];
            for (int i = 0; i < _themes.Length; i++)
            {
                values[i, 0] = Guid.NewGuid();
                values[i, 1] = _themes[i];
            }

            migrationBuilder.InsertData(
                schema: "ref",
                table: "Themes",
                columns: [ "Id", "Name" ],
                values: values);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "ref",
                table: "Themes",
                keyColumn: "Name",
                keyValues: _themes);
        }
    }
}

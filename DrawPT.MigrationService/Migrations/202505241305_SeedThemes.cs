using Microsoft.EntityFrameworkCore.Migrations;

namespace DrawPT.MigrationService.Migrations
{
    /// <inheritdoc />
    public partial class SeedThemes : Migration
    {
        private readonly string[] _themes =
        [
            "Under the Sea", "Space Adventure", "Jungle Safari",
            "Fairy Tale", "Wild West", "Ancient Egypt",
            "Anime", "Cyberpunk", "Miniaturecore", "Academia",
            "Steampunk", "Circus", "Horror Anime", "Cartoon",
            "Marvel Superheroes", "DC Superheroes",
            "Goblins", "Supernatural", "Fantasy", "Sci-Fi",
            "Aliens & UFOs", "Mystical", "Magical",
            "Invasions", "Time Travel", "Dinosaurs",
            "Nature and Wildlife", "Sports and Games",
            "Gundams"
        ];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ensure schema exists
            migrationBuilder.EnsureSchema(name: "ref");

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
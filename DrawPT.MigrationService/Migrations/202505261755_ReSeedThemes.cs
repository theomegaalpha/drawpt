using Microsoft.EntityFrameworkCore.Migrations;

namespace DrawPT.MigrationService.Migrations
{
    /// <inheritdoc />
    public partial class ReSeedThemes : Migration
    {
        private readonly string[] _themes =
        [
            "Under the Sea", "Space Adventure", "Jungle Safari",
            "Fairy Tale", "Wild West", "Ancient Egypt",
            "Anime", "Cyberpunk", "Miniaturecore", "Academia",
            "Steampunk", "Circus", "Horror Anime", "Cartoon",
            "Comic Book Superheroes", "Manga", "Video Games",
            "Goblins", "Supernatural", "Fantasy", "Sci-Fi",
            "Aliens & UFOs", "Mystical", "Post-Apocalyptic",
            "Invasions", "Time Travel", "Dinosaurs", "Magical",
            "Nature and Wildlife", "Sports and Games",
            "Gundam", "Medieval Times", "Robots and Androids",
            "Mythical Creatures", "Haunted Places", "Dreamscapes",
            "Noir", "Pirates", "Vikings", "Zombies", "Gods & Mythology",
            "Mad Science", "Secret Agents / Spies", "Lost Worlds",
            "Carnival Funfair", "Kaiju / Giant Monsters", "Witches & Wizards",
            "Music & Concerts", "Urban Legends", "Surreal Worlds",
            "Dystopian Societies", "Food Worlds", "Feudal Japan",
            "Elemental Magic", "Deep Sea Mysteries", "Ancient Rome",
            "Grand Heists", "Abstract Art", "Living Toys", "Gothic Style"
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
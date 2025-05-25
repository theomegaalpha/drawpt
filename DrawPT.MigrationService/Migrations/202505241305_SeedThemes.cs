using Microsoft.EntityFrameworkCore.Migrations;

namespace DrawPT.MigrationService.Migrations
{
    /// <inheritdoc />
    public partial class SeedThemes : Migration
    {
        private readonly string[] _themes =
        [
            "Under the Sea", "Space Adventure", "Jungle Safari",
            "Fairy Tale Kingdom", "Wild West", "Ancient Egypt",
            "Medieval Castle", "Futuristic City", "Enchanted Forest",
            "Circus Fun", "Dinosaur World", "Pirate's Treasure",
            "Arctic Expedition", "Desert Oasis", "Rainbow Paradise",
            "Haunted House", "Candy Land", "Robot Factory",
            "Superhero City", "Farming Life", "Deep Space",
            "Ocean Depths", "Mountain Peak", "Volcanic Island",
            "Crystal Cave", "Floating Islands", "Time Machine",
            "Magical Garden", "Castle in the Clouds", "Underground City"
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
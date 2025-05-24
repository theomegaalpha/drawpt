using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrawPT.MigrationService.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "ref");

            migrationBuilder.CreateTable(
                schema: "ref",
                name: "Adjectives",
                columns: table => new
                {
                    Adjective = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adjectives", x => x.Adjective);
                });

            migrationBuilder.CreateTable(
                schema: "ref",
                name: "Nouns",
                columns: table => new
                {
                    Noun = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nouns", x => x.Noun);
                });

            migrationBuilder.CreateTable(
                schema: "ref",
                name: "Themes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Themes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                schema: "ref",
                name: "Adjectives");
            migrationBuilder.DropTable(
                schema: "ref",
                name: "Nouns");
            migrationBuilder.DropTable(
                schema: "ref",
                name: "Themes");
        }
    }
}

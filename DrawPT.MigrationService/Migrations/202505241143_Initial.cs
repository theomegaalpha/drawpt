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
            migrationBuilder.CreateTable(
                name: "Adjectives",
                columns: table => new
                {
                    Adjective = table.Column<String>(type: "varchar(255)", nullable: false)
                });

            migrationBuilder.CreateTable(
                name: "Nouns",
                columns: table => new
                {
                    Noun = table.Column<String>(type: "varchar(255)", nullable: false)
                });

            migrationBuilder.CreateTable(
                name: "Themes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<String>(type: "varchar(255)", nullable: false)
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adjectives");
            migrationBuilder.DropTable(
                name: "Nouns");
        }
    }
}

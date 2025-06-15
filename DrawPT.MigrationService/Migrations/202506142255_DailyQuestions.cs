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

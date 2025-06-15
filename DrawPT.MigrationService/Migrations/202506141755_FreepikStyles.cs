using Microsoft.EntityFrameworkCore.Migrations;

namespace DrawPT.MigrationService.Migrations
{
    /// <inheritdoc />
    public partial class FreepikStyles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "game");

            migrationBuilder.CreateTable(
                schema: "game",
                name: "FreepikImageStyles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Style = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    NegativePrompt = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreepikImageStyles", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "FreepikImageStyles",
                columns: ["Id", "Style", "NegativePrompt"],
                values: new object[,]
                {
                    {
                        Guid.NewGuid(),
                        string.Empty,
                        "low quality, worst quality, normal quality, jpeg artifacts, ugly, duplicate, morbid, mutilated, extra fingers, fewer fingers, long neck, long body"
                    },
                    {
                        Guid.NewGuid(),
                        "anime",
                        "low quality, worst quality, normal quality, jpeg artifacts, ugly, duplicate, morbid, mutilated, extra fingers, fewer fingers, long neck, long body"
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FreepikImageStyles",
                schema: "game");
        }
    }
}

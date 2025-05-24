using Microsoft.EntityFrameworkCore.Migrations;
using OpenTelemetry;

#nullable disable

namespace DrawPT.MigrationService.Migrations
{
    /// <inheritdoc />
    public partial class SeedNouns : Migration
    {
        private readonly string[] _nouns =
        [
            "alligator", "ant", "bear", "bee", "bird",
            "camel", "cat", "cheetah", "chicken", "chimpanzee",
            "cow", "crocodile", "deer", "dog", "dolphin",
            "duck", "eagle", "elephant", "fish", "fly",
            "fox", "frog", "giraffe", "goat", "goldfish",
            "hamster", "hippopotamus", "horse", "kangaroo", "kitten",
            "lion", "lobster", "monkey", "octopus", "owl",
            "panda", "pig", "puppy", "rabbit", "rat",
            "scorpion", "seal", "shark", "sheep", "snail",
            "snake", "spider", "squirrel", "tiger", "turtle",
            "wolf", "zebra", "bat", "beetle", "buffalo",
            "butterfly", "crab", "crow", "dove", "dragonfly",
            "falcon", "flamingo", "grasshopper", "hawk",
            "hummingbird", "jellyfish", "koala", "ladybug", "lemur",
            "leopard", "lynx", "mole", "mosquito", "ostrich",
            "otter", "parrot", "peacock", "pelican", "penguin",
            "pigeon", "porcupine", "raccoon", "raven", "robin",
            "seagull", "sparrow", "swan", "toucan", "vulture",
            "walrus", "weasel", "whale", "woodpecker", "yak",
            "zebu"
        ];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ensure schema exists
            migrationBuilder.EnsureSchema(name: "ref");

            migrationBuilder.InsertData(
                schema: "ref",
                table: "Nouns",
                column: "Noun",
                values: _nouns);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                schema: "ref",
                name: "Nouns");
        }
    }
} 
namespace DrawPT.Data.Models
{
    public class GameConfiguration
    {
        public required int NumberOfQuestions { get; init; }

        public required int QuestionTimeout { get; init; }
        public required int ThemeTimeout { get; init; }
    }
}

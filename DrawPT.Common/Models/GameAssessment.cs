namespace DrawPT.Common.Models
{
    /// <summary>
    /// Represents the assessment of answers for a game question
    /// </summary>
    public class GameAssessment
    {
        /// <summary>
        /// Unique identifier for the assessment
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The question being assessed
        /// </summary>
        public string QuestionId { get; set; } = string.Empty;

        /// <summary>
        /// The assessed answers
        /// </summary>
        public List<GameAnswer> AssessedAnswers { get; set; } = new();

        /// <summary>
        /// When the assessment was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Any additional notes about the assessment
        /// </summary>
        public string Notes { get; set; } = string.Empty;
    }
}
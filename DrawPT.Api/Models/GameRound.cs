namespace DrawPT.Api.Models
{
    public class GameRound
    {
        public int RoundNumber { get; set; }
        public GameQuestion Question { get; set; } = new GameQuestion();
        public List<GameAnswer> Answers { get; set; } = [];
    }
}

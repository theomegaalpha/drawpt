using Microsoft.Identity.Client;

namespace DrawPT.Api.Models
{
    public class GameAnswerBase
    {
        public string? Guess { get; set; }
        public bool IsGambling { get; set; }
    }

    public class GameAnswer : GameAnswerBase
    {
        public string PlayerConnectionId { get; set; } = string.Empty;
        public int Score { get; set; }
        public int BonusPoints { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}

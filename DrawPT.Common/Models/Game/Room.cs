namespace DrawPT.Common.Models
{
    public class Room
    {
        public Guid Id { get; set; }
        public string Code { get; set; }

        public string Name { get; set; }

        public int PlayerLimit { get; set; }

        public List<Player> Players { get; set; }

        public bool IsGameStarted { get; set; }
        public bool IsPrivate { get; set; }

        public int CurrentRound { get; set; }
        public int TotalRounds {  get; set; }

        public Room()
        {
            Id = Guid.NewGuid();
            PlayerLimit = 6;
            Players = new List<Player>();
            TotalRounds = 5;
            RegenerateCode();
        }

        public void RegenerateCode()
        {
            var random = new Random();
            var joinCode = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            Code = joinCode;
        }
    }
}

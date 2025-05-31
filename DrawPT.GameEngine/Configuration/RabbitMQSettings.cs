namespace DrawPT.GameEngine.Configuration
{
    /// <summary>
    /// Configuration settings for RabbitMQ connection
    /// </summary>
    public class RabbitMQSettings
    {
        public string HostName { get; set; } = "localhost";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public int Port { get; set; } = 5672;
        public string VirtualHost { get; set; } = "/";
        public string ExchangeName { get; set; } = "game_events";
    }
} 
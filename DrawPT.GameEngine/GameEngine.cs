using DrawPT.Common.Interfaces;
using DrawPT.Common.Models;
using RabbitMQ.Client;

namespace DrawPT.GameEngine
{
    /// <summary>
    /// Core game engine implementation
    /// </summary>
    public class GameEngine : IGameEngine
    {
        private readonly ILogger<GameEngine> _logger;
        private readonly IModel _channel;
        private readonly string _gameId;
        private readonly GameConfiguration _configuration;
        private readonly IPlayerManager _playerManager;
        private readonly IRoundManager _roundManager;
        private readonly IQuestionManager _questionManager;

        public GameEngine(
            ILogger<GameEngine> logger,
            IConnection rabbitMqConnection,
            string gameId,
            GameConfiguration configuration,
            IPlayerManager playerManager,
            IRoundManager roundManager,
            IQuestionManager questionManager)
        {
            _logger = logger;
            _gameId = gameId;
            _configuration = configuration;
            _playerManager = playerManager;
            _roundManager = roundManager;
            _questionManager = questionManager;

            // Set up RabbitMQ channel
            _channel = rabbitMqConnection.CreateModel();
            _channel.ExchangeDeclare("game_events", ExchangeType.Topic, true);

            // Declare queue for game events
            _channel.QueueDeclare($"game_events_{gameId}", true, false, false);
            _channel.QueueBind($"game_events_{gameId}", "game_events", $"game.{gameId}.*");
        }

        /// <summary>
        /// Gets the unique identifier for this game instance
        /// </summary>
        public string GameId => _gameId;

        /// <summary>
        /// Gets the game configuration
        /// </summary>
        public GameConfiguration Configuration => _configuration;

        /// <summary>
        /// Gets the player manager for this game
        /// </summary>
        public IPlayerManager PlayerManager => _playerManager;

        /// <summary>
        /// Gets the round manager for this game
        /// </summary>
        public IRoundManager RoundManager => _roundManager;

        /// <summary>
        /// Gets the question manager for this game
        /// </summary>
        public IQuestionManager QuestionManager => _questionManager;

        /// <summary>
        /// Starts the game
        /// </summary>
        public async Task StartGameAsync()
        {
            await RoundManager.StartNewRoundAsync();
        }

        /// <summary>
        /// Ends the game
        /// </summary>
        public async Task EndGameAsync()
        {
            await RoundManager.EndRoundAsync();
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
} 
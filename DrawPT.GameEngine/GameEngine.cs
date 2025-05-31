using DrawPT.GameEngine.Events;
using DrawPT.GameEngine.Interfaces;
using DrawPT.GameEngine.Models;
using RabbitMQ.Client;

namespace DrawPT.GameEngine
{
    /// <summary>
    /// Base implementation of the game engine
    /// </summary>
    public class GameEngine : IGameEngine
    {
        private readonly IPlayerManager _playerManager;
        private readonly IRoundManager _roundManager;
        private readonly IQuestionManager _questionManager;
        private readonly IModel _channel;
        private readonly ILogger<GameEngine> _logger;

        private GameConfiguration _configuration = null!;
        private GameState _currentState = GameState.WaitingForPlayers;

        /// <summary>
        /// Unique identifier for the game instance
        /// </summary>
        public string GameId { get; }

        /// <summary>
        /// Current state of the game
        /// </summary>
        public GameState CurrentState => _currentState;

        public GameEngine(
            IPlayerManager playerManager,
            IRoundManager roundManager,
            IQuestionManager questionManager,
            IConnection rabbitMQConnection,
            ILogger<GameEngine> logger)
        {
            _playerManager = playerManager;
            _roundManager = roundManager;
            _questionManager = questionManager;
            _channel = rabbitMQConnection.CreateModel();
            _logger = logger;
            GameId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Starts a new game instance
        /// </summary>
        public async Task StartGameAsync(GameConfiguration configuration)
        {
            _configuration = configuration;
            _currentState = GameState.InProgress;

            _channel.BasicPublish();

            _logger.LogInformation("Game {GameId} started with configuration {@Configuration}", GameId, configuration);
        }

        /// <summary>
        /// Ends the current game instance
        /// </summary>
        public async Task EndGameAsync()
        {
            _currentState = GameState.Ended;

            var results = new GameResults
            {
                PlayerResults = (await _playerManager.GetPlayersAsync())
                    .Select(p => new PlayerResult
                    {
                        Id = p.Id,
                        ConnectionId = p.ConnectionId,
                        Username = p.Username,
                        FinalScore = p.Score
                    })
                    .ToList(),
                TotalRounds = _roundManager.CurrentRoundNumber,
                WasCompleted = true
            };

            await _eventBus.PublishAsync(new GameEndedEvent
            {
                GameId = GameId,
                Results = results
            });

            _logger.LogInformation("Game {GameId} ended with results {@Results}", GameId, results);
        }

        /// <summary>
        /// Adds a player to the game
        /// </summary>
        public async Task<bool> AddPlayerAsync(string connectionId, Player player)
        {
            if (_currentState != GameState.WaitingForPlayers)
            {
                _logger.LogWarning($"Cannot add player {player.Id} to game {GameId} in state {_currentState}");
                return false;
            }

            var addedPlayer = await _playerManager.AddPlayerAsync(connectionId, player);
            if (addedPlayer != null)
            {
                await _eventBus.PublishAsync(new PlayerJoinedEvent
                {
                    GameId = GameId,
                    Player = addedPlayer
                });

                _logger.LogInformation($"Player {player.Id} joined game {GameId}");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a player from the game
        /// </summary>
        public async Task RemovePlayerAsync(string connectionId)
        {
            var player = await _playerManager.GetPlayerAsync(connectionId);
            if (player != null)
            {
                await _playerManager.RemovePlayerAsync(connectionId);
                await _eventBus.PublishAsync(new PlayerLeftEvent
                {
                    GameId = GameId,
                    Player = player
                });

                _logger.LogInformation($"Player {player.Id} left game {GameId}");
            }
        }

        /// <summary>
        /// Processes the next round of the game
        /// </summary>
        public async Task ProcessNewRoundAsync()
        {
            if (_currentState != GameState.InProgress)
            {
                _logger.LogWarning($"Cannot process next round in game {GameId} in state {_currentState}");
                return;
            }

            var round = await _roundManager.StartNewRoundAsync();
            await _eventBus.PublishAsync(new RoundStartedEvent
            {
                GameId = GameId,
                Round = round
            });

            // Process the round
            await _roundManager.EndRoundAsync();
            await _eventBus.PublishAsync(new RoundEndedEvent
            {
                GameId = GameId,
                Round = round
            });

            _logger.LogInformation($"Round {_roundManager.CurrentRoundNumber} completed in game {GameId}");
        }

        /// <summary>
        /// Gets the current game configuration
        /// </summary>
        public GameConfiguration GetConfiguration() => _configuration;
    }
}
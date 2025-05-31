using DrawPT.GameEngine.Interfaces;
using DrawPT.GameEngine.Models;
using RabbitMQ.Client;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace DrawPT.GameEngine.Managers
{
    /// <summary>
    /// Manages player-related operations within the game
    /// </summary>
    public class PlayerManager : IPlayerManager
    {
        private readonly ILogger<PlayerManager> _logger;
        private readonly IModel _channel;
        private readonly string _gameId;
        private readonly ConcurrentDictionary<string, Player> _players = new();
        private readonly GameConfiguration _configuration;

        public PlayerManager(
            ILogger<PlayerManager> logger,
            IConnection rabbitMqConnection,
            string gameId,
            GameConfiguration configuration)
        {
            _logger = logger;
            _gameId = gameId;
            _configuration = configuration;

            // Set up RabbitMQ channel
            _channel = rabbitMqConnection.CreateModel();
            _channel.ExchangeDeclare("game_events", ExchangeType.Topic, true);

            // Declare queues for player events
            _channel.QueueDeclare($"player_events_{gameId}", true, false, false);
            _channel.QueueBind($"player_events_{gameId}", "game_events", $"game.{gameId}.player.*");
        }

        /// <summary>
        /// Gets the current number of players in the game
        /// </summary>
        public int PlayerCount => _players.Count;

        /// <summary>
        /// Adds a new player to the game
        /// </summary>
        public Player AddPlayer(string connectionId, Player player)
        {
            if (IsGameFull())
            {
                _logger.LogWarning($"Game {_gameId} is full, cannot add player {player.Id}");
                return null!;
            }

            player.ConnectionId = connectionId;
            if (_players.TryAdd(connectionId, player))
            {
                // Publish player joined event to RabbitMQ
                var message = new
                {
                    EventType = "PlayerJoined",
                    GameId = _gameId,
                    Player = player
                };

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
                _channel.BasicPublish(
                    exchange: "game_events",
                    routingKey: $"game.{_gameId}.player.joined",
                    basicProperties: null,
                    body: body);

                _logger.LogInformation($"Player {player.Id} added to game {_gameId}");
                return player;
            }

            return null!;
        }

        /// <summary>
        /// Removes a player from the game
        /// </summary>
        public void RemovePlayer(string connectionId)
        {
            if (_players.TryRemove(connectionId, out var player))
            {
                // Publish player left event to RabbitMQ
                var message = new
                {
                    EventType = "PlayerLeft",
                    GameId = _gameId,
                    Player = player
                };

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
                _channel.BasicPublish(
                    exchange: "game_events",
                    routingKey: $"game.{_gameId}.player.left",
                    basicProperties: null,
                    body: body);

                _logger.LogInformation($"Player {player.Id} removed from game {_gameId}");
            }
        }

        /// <summary>
        /// Gets all current players in the game
        /// </summary>
        public Task<IEnumerable<Player>> GetPlayersAsync()
        {
            return Task.FromResult(_players.Values.AsEnumerable());
        }

        /// <summary>
        /// Updates a player's score
        /// </summary>
        public void UpdatePlayerScore(string connectionId, int score)
        {
            if (_players.TryGetValue(connectionId, out var player))
            {
                player.Score = score;

                // Publish score update event to RabbitMQ
                var message = new
                {
                    EventType = "PlayerScoreUpdated",
                    GameId = _gameId,
                    PlayerId = player.Id,
                    NewScore = score
                };

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
                _channel.BasicPublish(
                    exchange: "game_events",
                    routingKey: $"game.{_gameId}.player.score",
                    basicProperties: null,
                    body: body);

                _logger.LogInformation($"Updated score for player {player.Id} in game {_gameId} to {score}");
            }
        }

        /// <summary>
        /// Gets a specific player by their connection ID
        /// </summary>
        public Player? GetPlayer(string connectionId)
        {
            return _players.TryGetValue(connectionId, out var player) ? player : null;
        }

        /// <summary>
        /// Gets all players in the game
        /// </summary>
        public IEnumerable<Player> GetPlayers()
        {
            return _players.Values;
        }

        /// <summary>
        /// Checks if the game has reached maximum player capacity
        /// </summary>
        public bool IsGameFull()
        {
            return _players.Count >= _configuration.MaxPlayers;
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}
using System.Text;
using System.Text.Json;
using DrawPT.GameEngine.Events;
using DrawPT.GameEngine.Interfaces;
using DrawPT.GameEngine.Helpers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DrawPT.GameEngine.Services
{
    /// <summary>
    /// Service for handling game events using RabbitMQ
    /// </summary>
    public class GameEventService : IGameEventService, IDisposable
    {
        private readonly ILogger<GameEventService> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchangeName;
        private readonly Dictionary<GameEventType, IGameEventHandler> _eventHandlers;

        public GameEventService(
            ILogger<GameEventService> logger,
            IConnection connection,
            IEnumerable<IGameEventHandler> eventHandlers)
        {
            _logger = logger;
            _connection = connection;
            _channel = connection.CreateModel();
            _exchangeName = GameEventRouting.Exchange;
            _eventHandlers = eventHandlers.ToDictionary(h => h.EventType);

            // Declare the exchange
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, true);

            // Set up consumer
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += OnMessageReceived;
            _channel.BasicConsume(queue: "game_events", autoAck: true, consumer: consumer);
        }

        /// <summary>
        /// Publishes a game event to RabbitMQ
        /// </summary>
        public void PublishEvent(IGameEvent gameEvent)
        {
            try
            {
                var routingKey = RabbitMQHelpers.GetRoutingKey(gameEvent);
                var message = JsonSerializer.Serialize(gameEvent);
                var body = Encoding.UTF8.GetBytes(message);

                _channel.BasicPublish(
                    exchange: _exchangeName,
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body);

                _logger.LogInformation("Published event {EventType} with routing key {RoutingKey}", 
                    gameEvent.EventType, routingKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing event {EventType}", gameEvent.EventType);
                throw;
            }
        }

        /// <summary>
        /// Subscribes to events for a specific game
        /// </summary>
        public void SubscribeToGame(string gameId)
        {
            var queueName = GameEventRouting.CreateGameQueueName(gameId);
            var bindingPattern = GameEventRouting.CreateGameBindingPattern(gameId);

            _channel.QueueDeclare(queueName, true, false, false);
            _channel.QueueBind(queueName, _exchangeName, bindingPattern);

            _logger.LogInformation("Subscribed to game events for game {GameId}", gameId);
        }

        /// <summary>
        /// Subscribes to events for a specific player
        /// </summary>
        public void SubscribeToPlayer(string gameId, string playerId)
        {
            var queueName = GameEventRouting.CreatePlayerQueueName(gameId, playerId);
            var bindingPattern = GameEventRouting.CreatePlayerBindingPattern(gameId);

            _channel.QueueDeclare(queueName, true, false, false);
            _channel.QueueBind(queueName, _exchangeName, bindingPattern);

            _logger.LogInformation("Subscribed to player events for player {PlayerId} in game {GameId}", 
                playerId, gameId);
        }

        /// <summary>
        /// Subscribes to events for a specific round
        /// </summary>
        public void SubscribeToRound(string gameId, int roundNumber)
        {
            var queueName = GameEventRouting.CreateRoundQueueName(gameId, roundNumber);
            var bindingPattern = GameEventRouting.CreateRoundBindingPattern(gameId);

            _channel.QueueDeclare(queueName, true, false, false);
            _channel.QueueBind(queueName, _exchangeName, bindingPattern);

            _logger.LogInformation("Subscribed to round events for round {RoundNumber} in game {GameId}", 
                roundNumber, gameId);
        }

        /// <summary>
        /// Subscribes to events for a specific question
        /// </summary>
        public void SubscribeToQuestion(string gameId, string questionId)
        {
            var queueName = GameEventRouting.CreateQuestionQueueName(gameId, questionId);
            var bindingPattern = GameEventRouting.CreateQuestionBindingPattern(gameId);

            _channel.QueueDeclare(queueName, true, false, false);
            _channel.QueueBind(queueName, _exchangeName, bindingPattern);

            _logger.LogInformation("Subscribed to question events for question {QuestionId} in game {GameId}", 
                questionId, gameId);
        }


        private void OnMessageReceived(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                var eventType = GameEventRouting.GetEventTypeFromRoutingKey(e.RoutingKey);
                
                if (_eventHandlers.TryGetValue(eventType, out var handler))
                {
                    handler.HandleEvent(message);
                }
                else
                {
                    _logger.LogWarning("No handler found for event type {EventType}", eventType);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message with routing key {RoutingKey}", e.RoutingKey);
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
} 
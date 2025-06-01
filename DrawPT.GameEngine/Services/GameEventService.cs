using System.Text;
using System.Text.Json;
using DrawPT.Common.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DrawPT.GameEngine.Services
{
    /// <summary>
    /// Service for handling game events through RabbitMQ
    /// </summary>
    public class GameEventService : IDisposable
    {
        private readonly IConnection _rabbitMQConnection;
        private readonly ILogger<GameEventService> _logger;
        private readonly IModel _channel;
        private readonly string _exchangeName;
        private readonly string _queueName;

        public GameEventService(
            IConfiguration configuration,
            IConnection rabbitMQConnection,
            ILogger<GameEventService> logger)
        {
            _rabbitMQConnection = rabbitMQConnection;
            _logger = logger;
            _channel = _rabbitMQConnection.CreateModel();
            _exchangeName = configuration.GetValue<string>("RabbitMQ:ExchangeName") ?? "game_events";
            _queueName = $"game_engine_{Guid.NewGuid()}";

            SetupRabbitMQ();
        }

        private void SetupRabbitMQ()
        {
            // Declare exchange
            _channel.ExchangeDeclare(
                exchange: _exchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            // Declare queue
            _channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            // Bind queue to exchange with routing key pattern
            _channel.QueueBind(
                queue: _queueName,
                exchange: _exchangeName,
                routingKey: "game.*");

            // Set up consumer
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;

                    _logger.LogInformation("Received message: {Message} with routing key: {RoutingKey}", 
                        message, routingKey);

                    // TODO: Handle different event types based on routing key
                    // This will be implemented when we have specific event handlers
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                }
            };

            _channel.BasicConsume(
                queue: _queueName,
                autoAck: true,
                consumer: consumer);
        }

        /// <summary>
        /// Publishes a game event to RabbitMQ
        /// </summary>
        public void PublishEvent(IGameEvent gameEvent)
        {
            try
            {
                var message = JsonSerializer.Serialize(gameEvent);
                var body = Encoding.UTF8.GetBytes(message);

                var routingKey = $"game.{gameEvent.GetType().Name.ToLower()}";

                _channel.BasicPublish(
                    exchange: _exchangeName,
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body);

                _logger.LogInformation("Published event: {EventType} with routing key: {RoutingKey}", 
                    gameEvent.GetType().Name, routingKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing event");
                throw;
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
} 
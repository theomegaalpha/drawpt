using DrawPT.Common.Models;
using Microsoft.Extensions.Caching.Distributed;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;


namespace DrawPT.GameEngine
{
    public class GameOrchestrator : BackgroundService
    {
        private readonly ILogger<GameOrchestrator> _logger;
        private readonly IConfiguration _config;
        private readonly IServiceProvider _serviceProvider;
        private IConnection? _messageConnection;
        private IModel? _messageChannel;
        private readonly IDistributedCache _cache;
        private EventingBasicConsumer consumer;

        public GameOrchestrator(ILogger<GameOrchestrator> logger, IConfiguration config,
            IServiceProvider serviceProvider, IConnection? messageConnection, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
            _config = config;
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string exchangeName = "matchmaking_events";

            _messageConnection = _serviceProvider.GetRequiredService<IConnection>();
            _messageChannel = _messageConnection.CreateModel();

            _messageChannel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic, durable: false);

            string queueName = _messageChannel.QueueDeclare().QueueName;

            _messageChannel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: "room.created");

            consumer = new EventingBasicConsumer(_messageChannel);
            consumer.Received += ProcessMessageAsync;

            // 5. Start consuming
            _messageChannel.BasicConsume(
                queue: queueName,
                autoAck: true,
                consumer: consumer);

            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            consumer.Received -= ProcessMessageAsync;
            _messageChannel?.Dispose();
        }

        private void ProcessMessageAsync(object? sender, BasicDeliverEventArgs args)
        {
            string roomCode = Encoding.UTF8.GetString(args.Body.ToArray());


            var gameState = new GameState() { RoomCode = roomCode};

            string serializedGameState = JsonSerializer.Serialize(gameState);

            // Store the game state in Redis cache with a 1-hour expiration
            _cache.SetString($"room:{roomCode}", serializedGameState, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) });
        }
    }
}
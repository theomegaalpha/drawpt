using DrawPT.Common.Configuration;
using DrawPT.GameEngine.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace DrawPT.GameEngine.BackgroundWorkers;

public class GameEventListener : BackgroundService
{
    private readonly ILogger<GameEventListener> _logger;
    private readonly IModel _channel;
    private readonly IGameEngine _gameEngine;

    public GameEventListener(
        ILogger<GameEventListener> logger,
        IConnection rabbitMqConnection,
        IGameEngine gameEngine)
    {
        _logger = logger;
        _channel = rabbitMqConnection.CreateModel();
        _gameEngine = gameEngine;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Declare exchange and queue
        _channel.ExchangeDeclare(GameMQ.ExchangeName, ExchangeType.Topic);
        _channel.QueueDeclare(GameMQ.QueueName);
        _channel.QueueBind(GameMQ.QueueName, GameMQ.ExchangeName, GameMQ.RoutingKey);

        // Set up consumer
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;
            var roomCode = routingKey.Split('.')[1];

            _logger.LogInformation($"Received message with routing key: {routingKey}");
            _logger.LogInformation($"Message content: {message}");

            // Handle game started event
            if (routingKey.EndsWith(GameMQ.GameStart))
            {
                _logger.LogInformation($"Game started event received for room: {routingKey.Split('.')[1]}");

                // TODO: implement way of tracking used threads and clean up when game ends
                _ = Task.Run(async () => await _gameEngine.PlayGameAsync(roomCode));
            }
        };

        _channel.BasicConsume(
            queue: GameMQ.QueueName,
            autoAck: true,
            consumer: consumer);

        _logger.LogInformation("Started consuming from game queue");

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        base.Dispose();
    }
}
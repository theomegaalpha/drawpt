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
    private readonly IGameSession _gameEngine;

    public GameEventListener(
        ILogger<GameEventListener> logger,
        IConnection rabbitMqConnection,
        IGameSession gameEngine)
    {
        _logger = logger;
        _channel = rabbitMqConnection.CreateModel();
        _gameEngine = gameEngine;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Declare exchange and queue
        _channel.ExchangeDeclare(ApiMasterMQ.ExchangeName, ExchangeType.Topic);
        _channel.QueueDeclare(ApiMasterMQ.QueueName);
        _channel.QueueBind(ApiMasterMQ.QueueName, ApiMasterMQ.ExchangeName, ApiMasterMQ.RoutingKey);

        // Set up consumer
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;
            var roomCode = routingKey.Split('.')[1];

            _logger.LogInformation($"Received message with routing key: {routingKey}");
            _logger.LogInformation($"Message content: {message}");

            // Handle game started event
            if (routingKey.EndsWith(ApiMasterMQ.GameStartedAction))
            {
                _logger.LogInformation($"Game started event received for room: {roomCode}");

                // TODO: implement way of tracking used threads and clean up when game ends
                _ = Task.Run(async () => await _gameEngine.PlayGameAsync(roomCode));
            }
        };

        _channel.BasicConsume(
            queue: ApiMasterMQ.QueueName,
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
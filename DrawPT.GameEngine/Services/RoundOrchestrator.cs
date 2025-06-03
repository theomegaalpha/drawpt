using DrawPT.Common.Configuration;
using DrawPT.Common.Models.Game;
using DrawPT.GameEngine.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;

namespace DrawPT.GameEngine.Services;

public class RoundOrchestrator : IRoundOrchestrator
{
    private readonly IModel _channel;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _pendingRequests;
    private readonly ILogger<RoundOrchestrator> _logger;

    public RoundOrchestrator(
        IConnection rabbitMqConnection,
        ILogger<RoundOrchestrator> logger)
    {
        _channel = rabbitMqConnection.CreateModel();
        _channel.ExchangeDeclare(GameResponseMQ.ExchangeName, ExchangeType.Topic);
        _channel.ExchangeDeclare(ClientBroadcastMQ.ExchangeName, ExchangeType.Topic);
        _channel.ExchangeDeclare(ClientInteractionMQ.ExchangeName, ExchangeType.Topic);
        _pendingRequests = new ConcurrentDictionary<string, TaskCompletionSource<string>>();
        _logger = logger;

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var response = Encoding.UTF8.GetString(body);
            var correlationId = ea.BasicProperties.CorrelationId;

            if (_pendingRequests.TryRemove(correlationId, out var tcs))
            {
                tcs.TrySetResult(response);
            }
        };
        _channel.QueueDeclare(queue: GameResponseMQ.QueueName);
        _channel.QueueBind(GameResponseMQ.QueueName,
            GameResponseMQ.ExchangeName, GameResponseMQ.RoutingKey);
        _channel.BasicConsume(queue: GameResponseMQ.QueueName, autoAck: true, consumer: consumer);
    }


    public async Task<string> RequestUserInputAsync(string requestPayload, string connectionId, int timeoutMilliseconds)
    {
        var correlationId = Guid.NewGuid().ToString();
        var replyQueue = GameResponseMQ.QueueName;

        var properties = _channel.CreateBasicProperties();
        properties.CorrelationId = correlationId;
        properties.ReplyTo = replyQueue;

        var messageBytes = Encoding.UTF8.GetBytes(requestPayload);

        // Create a TaskCompletionSource to wait for the response
        var tcs = new TaskCompletionSource<string>();
        _pendingRequests[correlationId] = tcs;

        _channel.BasicPublish(exchange: ClientInteractionMQ.ExchangeName,
                              routingKey: ClientInteractionMQ.RoutingKeys.AskTheme(connectionId),
                              basicProperties: properties,
                              body: messageBytes);

        Console.WriteLine($"[x] Sent request with CorrelationId: {correlationId}");

        // Wait synchronously for response or timeout
        var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(timeoutMilliseconds));

        if (completedTask == tcs.Task)
            return tcs.Task.Result;

        _pendingRequests.TryRemove(correlationId, out _);
        _logger.LogDebug("No response received from client within the timeout period.");
        return string.Empty;
    }

    public async Task<GameTheme> SelectThemeAsync(string playerConnectionId)
    {
        return await Task.FromResult(new GameTheme() { Id = "id", Name = "anime" });
    }

    public async Task<GameQuestion> GenerateQuestionAsync(GameTheme theme)
    {
        //var question = await _aiClient.GenerateGameQuestionAsync(theme.Name);
        return new GameQuestion();
    }

    public async Task<List<PlayerAnswer?>> CollectAnswersAsync(GameQuestion question)
    {
        return await Task.FromResult<List<PlayerAnswer>>(new List<PlayerAnswer?>());
    }

    public async Task<List<PlayerAnswer>> AssessAnswersAsync(GameQuestion question, List<PlayerAnswer> answers)
    {
        return new List<PlayerAnswer>();
    }

    public Task<bool> IsRoundCompleteAsync(GameRound round)
    {
        return Task.FromResult(true);
    }
} 
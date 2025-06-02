using DrawPT.Common.Configuration;
using DrawPT.Common.Models;
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

    public RoundOrchestrator(
        IConnection rabbitMqConnection)
    {
        _channel = rabbitMqConnection.CreateModel();
        _channel.ExchangeDeclare(ClientBroadcastMQ.ExchangeName, ExchangeType.Topic);
        _pendingRequests = new ConcurrentDictionary<string, TaskCompletionSource<string>>();

        var replyQueue = GameResponseMQ.QueueName;
        _channel.QueueDeclare(queue: replyQueue);

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
        _channel.BasicConsume(queue: replyQueue, autoAck: true, consumer: consumer);
    }


    public async Task<string> RequestUserInputAsync(string requestPayload, int timeoutMilliseconds)
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

        _channel.BasicPublish(exchange: GameRequestMQ.QueueName,
                              routingKey: GameRequestMQ.RoutingKey,
                              basicProperties: properties,
                              body: messageBytes);

        Console.WriteLine($"[x] Sent request with CorrelationId: {correlationId}");

        // Wait synchronously for response or timeout
        var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(timeoutMilliseconds));

        if (completedTask == tcs.Task)
        {
            return tcs.Task.Result; // Response received
        }
        else
        {
            _pendingRequests.TryRemove(correlationId, out _); // Cleanup
            throw new TimeoutException("No response received within the timeout period.");
        }
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
        return await Task.FromResult(new List<PlayerAnswer?>());
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
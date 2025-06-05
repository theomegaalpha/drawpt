using DrawPT.Common.Configuration;
using DrawPT.Common.Interfaces.Game;
using DrawPT.Common.Models;
using DrawPT.Common.Models.Game;
using DrawPT.GameEngine.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace DrawPT.GameEngine.Services;

public class GameCommunicationService : IGameCommunicationService
{
    private readonly IModel _channel;
    private readonly IThemeService _themeService;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _pendingRequests;
    private readonly ILogger<GameCommunicationService> _logger;

    public GameCommunicationService(
        IThemeService themeService,
        IConnection rabbitMqConnection,
        ILogger<GameCommunicationService> logger)
    {
        _themeService = themeService;

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

    public void BroadcastGameEvent(string roomCode, string gameAction, object? message = null)
    {
        var routingKey = $"{ClientBroadcastMQ.QueueName}.{roomCode}.{gameAction}";
        var payloadJson = JsonSerializer.Serialize(message);
        var messageBytes = Encoding.UTF8.GetBytes(payloadJson);
        _channel.BasicPublish(exchange: ClientBroadcastMQ.ExchangeName,
                              routingKey: routingKey,
                              basicProperties: null,
                              body: messageBytes);
        _logger.LogDebug($"[{roomCode}] Broadcasted game event: {gameAction} with message: {message}");
    }

    public async Task<string> AskPlayerTheme(Player player, int timeoutInSeconds)
    {
        var themes = _themeService.GetRandomThemes();
        var themesJson = JsonSerializer.Serialize(themes);
        var routingKey = ClientInteractionMQ.RoutingKeys.AskTheme(player.ConnectionId);
        var selectedTheme = await RequestUserInputAsync(routingKey, themesJson, player.ConnectionId, timeoutInSeconds * 1000);

        if (string.IsNullOrEmpty(selectedTheme))
        {
            _logger.LogDebug($"[{player.RoomCode}] No theme selected by player {player.Id} within the timeout period.");
            return themes[new Random().Next(themes.Count)];
        }
        return selectedTheme;
    }

    public async Task<PlayerAnswer> AskPlayerQuestion(Player player, GameQuestion question, int timeoutInSeconds)
    {
        var questionJson = JsonSerializer.Serialize(question);
        var routingKey = ClientInteractionMQ.RoutingKeys.AskQuestion(player.ConnectionId);
        var answerString = await RequestUserInputAsync(routingKey, questionJson, player.ConnectionId, timeoutInSeconds * 1000);
        PlayerAnswerBase? answerBase;
        try 
        {
            answerBase = JsonSerializer.Deserialize<PlayerAnswerBase>(answerString);
        }
        catch
        {
            answerBase = null;
        }

        var answer = new PlayerAnswer();
        if (answerBase == null)
        {
            _logger.LogDebug($"[{player.RoomCode}] No answer given by player {player.Id} within the timeout period.");
            answer.Reason = "No answer provided within the timeout period.";
            return answer;
        }

        answer.ConnectionId = player.ConnectionId;
        answer.IsGambling = answerBase.IsGambling;
        answer.Guess = answerBase.Guess;
        return answer;
    }


    private async Task<string> RequestUserInputAsync(string routingKey, string requestPayload, string connectionId, int timeoutMilliseconds)
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
                              routingKey: routingKey,
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

    public Task BroadcastGameEvent(string roomCode, string message)
    {
        return Task.CompletedTask;
    }
}
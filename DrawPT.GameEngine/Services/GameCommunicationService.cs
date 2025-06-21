using DrawPT.Common.Configuration;
using DrawPT.Common.Interfaces.Game;
using DrawPT.Common.Models;
using DrawPT.Common.Models.Game;
using DrawPT.GameEngine.Interfaces;
using Azure.Messaging.ServiceBus;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;

namespace DrawPT.GameEngine.Services;

public class GameCommunicationService : IGameCommunicationService
{
    private readonly IThemeService _themeService;
    private readonly ServiceBusClient _sbClient;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _pendingRequests;
    private readonly ILogger<GameCommunicationService> _logger;

    public GameCommunicationService(
        IThemeService themeService,
        ServiceBusClient sbClient,
        ILogger<GameCommunicationService> logger)
    {
        _themeService = themeService;
        _sbClient = sbClient;
        _pendingRequests = new ConcurrentDictionary<string, TaskCompletionSource<string>>();
        _logger = logger;
    }

    public void BroadcastGameEvent(string roomCode, string gameAction, object? message = null)
    {
        // Build the Service Bus message payload
        var payload = new
        {
            Action = gameAction,
            Payload = message
        };
        string json = JsonSerializer.Serialize(payload);

        // Send to the 'gameEngine' queue
        ServiceBusSender sender = _sbClient.CreateSender("gameEngine");
        var sbMessage = new ServiceBusMessage(json)
        {
            SessionId = roomCode
        };
        // Synchronously send the message
        sender.SendMessageAsync(sbMessage).GetAwaiter().GetResult();

        _logger.LogDebug($"[{roomCode}] Broadcasted game event via SB: {gameAction} with message: {message}");
    }

    public async Task<string> AskPlayerThemeAsync(Player player, int timeoutInSeconds)
    {
        // Prepare request message for Service Bus
        var themes = _themeService.GetRandomThemes();
        var requestObj = new { Action = GameEngineRequestMQ.Theme, Payload = themes };
        var requestPayload = JsonSerializer.Serialize(requestObj);

        // Send via Service Bus and await response
        var correlationId = Guid.NewGuid().ToString();
        var replyQueue = "gameEngineResponse";
        var processor = _sbClient.CreateProcessor(replyQueue, new ServiceBusProcessorOptions { AutoCompleteMessages = false });
        var tcs = new TaskCompletionSource<string>();
        processor.ProcessMessageAsync += async args =>
        {
            if (args.Message.CorrelationId == correlationId)
            {
                tcs.TrySetResult(args.Message.Body.ToString());
                await args.CompleteMessageAsync(args.Message);
            }
        };
        processor.ProcessErrorAsync += args => { _logger.LogError(args.Exception, "Error processing SB response"); return Task.CompletedTask; };
        await processor.StartProcessingAsync();

        var sender = _sbClient.CreateSender("gameEngineRequest");
        var requestMsg = new ServiceBusMessage(requestPayload)
        {
            SessionId = player.ConnectionId,
            CorrelationId = correlationId,
            ReplyTo = replyQueue
        };
        await sender.SendMessageAsync(requestMsg);

        // Wait for response or timeout
        var delay = Task.Delay(TimeSpan.FromSeconds(timeoutInSeconds + 5));
        var completed = await Task.WhenAny(tcs.Task, delay);
        await processor.StopProcessingAsync();

        string selectedTheme = completed == tcs.Task
            ? tcs.Task.Result
            : themes[new Random().Next(themes.Count)];

        _logger.LogDebug($"[{player.RoomCode}] Theme selected for player {player.Id}: {selectedTheme}");
        BroadcastGameEvent(player.RoomCode, GameEngineBroadcastMQ.PlayerThemeSelectedAction, selectedTheme);
        return selectedTheme;
    }

    public async Task<PlayerAnswer> AskPlayerQuestionAsync(Player player, GameQuestion question, int timeoutInSeconds)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // Build SB request message
        var requestObj = new { Action = GameEngineRequestMQ.Question, Payload = question };
        var requestPayload = JsonSerializer.Serialize(requestObj);

        var correlationId = Guid.NewGuid().ToString();
        var replyQueue = "gameEngineResponse";
        var processor = _sbClient.CreateProcessor(replyQueue, new ServiceBusProcessorOptions { AutoCompleteMessages = false });
        var tcs = new TaskCompletionSource<string>();
        processor.ProcessMessageAsync += async args =>
        {
            if (args.Message.CorrelationId == correlationId)
            {
                tcs.TrySetResult(args.Message.Body.ToString());
                await args.CompleteMessageAsync(args.Message);
            }
        };
        processor.ProcessErrorAsync += args => { _logger.LogError(args.Exception, "Error processing SB response"); return Task.CompletedTask; };
        await processor.StartProcessingAsync();

        var sender = _sbClient.CreateSender("gameEngineRequest");
        var requestMsg = new ServiceBusMessage(requestPayload)
        {
            SessionId = player.ConnectionId,
            CorrelationId = correlationId,
            ReplyTo = replyQueue
        };
        await sender.SendMessageAsync(requestMsg);

        // Wait for response or timeout
        var delay = Task.Delay(TimeSpan.FromSeconds(timeoutInSeconds + 5));
        var completed = await Task.WhenAny(tcs.Task, delay);
        await processor.StopProcessingAsync();

        string answerString = completed == tcs.Task ? tcs.Task.Result : string.Empty;

        // Deserialize and build PlayerAnswer
        PlayerAnswerBase? answerBase = null;
        try { answerBase = JsonSerializer.Deserialize<PlayerAnswerBase>(answerString); } catch { }

        var answer = new PlayerAnswer();
        if (answerBase == null)
        {
            _logger.LogDebug($"[{player.RoomCode}] No answer given by player {player.Id} within the timeout period.");
            answer.Reason = "No answer provided within the timeout period.";
        }
        else
        {
            answer.IsGambling = answerBase.IsGambling;
            answer.Guess = answerBase.Guess;
        }

        stopwatch.Stop();
        answer.BonusPoints = CalculateBonusPoints(stopwatch.Elapsed.TotalSeconds);
        answer.ConnectionId = player.ConnectionId;
        answer.PlayerId = player.Id;
        answer.Username = player.Username;
        answer.Avatar = player.Avatar;

        BroadcastGameEvent(player.RoomCode, GameEngineBroadcastMQ.PlayerAnsweredAction, answer);
        return answer;
    }

    int CalculateBonusPoints(double elapsedTime)
    {
        if (elapsedTime <= 3)
            return 5;
        if (elapsedTime >= 15)
            return 0;
        // Linear decrease from 5 to 0 between 3 and 15 seconds
        return (int) Math.Ceiling(5 * (1 - (elapsedTime - 3) / 12));
    }
}

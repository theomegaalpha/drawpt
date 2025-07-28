using DrawPT.Common.ServiceBus;
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
    private readonly ServiceBusProcessor _replyProcessor;

    public GameCommunicationService(
        IThemeService themeService,
        ServiceBusClient sbClient,
        ILogger<GameCommunicationService> logger)
    {
        _themeService = themeService;
        _sbClient = sbClient;
        _pendingRequests = new ConcurrentDictionary<string, TaskCompletionSource<string>>();
        _logger = logger;

        // Initialize centralized reply processor
        _replyProcessor = _sbClient.CreateProcessor("gameEngineResponse", new ServiceBusProcessorOptions { AutoCompleteMessages = false, MaxConcurrentCalls = 8 });
        _replyProcessor.ProcessMessageAsync += ReplyHandlerAsync;
        _replyProcessor.ProcessErrorAsync += args => { _logger.LogError(args.Exception, "Error processing SB response"); return Task.CompletedTask; };
        _replyProcessor.StartProcessingAsync();
    }

    private async Task ReplyHandlerAsync(ProcessMessageEventArgs args)
    {
        if (_pendingRequests.TryRemove(args.Message.CorrelationId, out var tcs))
        {
            tcs.TrySetResult(args.Message.Body.ToString());
            await args.CompleteMessageAsync(args.Message);
        }
        else
        {
            await args.AbandonMessageAsync(args.Message);
        }
    }

    public async Task BroadcastGameEventAsync(string roomCode, string gameAction, object? message = null)
    {
        var payload = new
        {
            Action = gameAction,
            Payload = message
        };
        string json = JsonSerializer.Serialize(payload);

        ServiceBusSender sender = _sbClient.CreateSender(GameEngineQueue.Name);
        var sbMessage = new ServiceBusMessage(json)
        {
            SessionId = roomCode
        };
        await sender.SendMessageAsync(sbMessage);

        _logger.LogDebug($"[{roomCode}] Broadcasted game event via SB: {gameAction} with message: {message}");
    }

    public async Task<string> AskPlayerThemeAsync(Player player, int timeoutInSeconds)
    {
        // Prepare request message for Service Bus
        var themes = _themeService.GetRandomThemes();
        var requestObj = new { player.RoomCode, Action = GameEngineRequests.Theme, Payload = themes };
        var requestPayload = JsonSerializer.Serialize(requestObj);

        // Send via Service Bus and await response
        var correlationId = Guid.NewGuid().ToString();
        var tcs = new TaskCompletionSource<string>();
        _pendingRequests[correlationId] = tcs;

        var sender = _sbClient.CreateSender("gameEngineRequest");
        var requestMsg = new ServiceBusMessage(requestPayload)
        {
            SessionId = player.ConnectionId,
            CorrelationId = correlationId,
            ReplyTo = "gameEngineResponse"
        };
        await sender.SendMessageAsync(requestMsg);

        // Wait for response or timeout
        var delay = Task.Delay(TimeSpan.FromSeconds(timeoutInSeconds + 5));
        var completed = await Task.WhenAny(tcs.Task, delay);
        _pendingRequests.TryRemove(correlationId, out _);

        string selectedTheme = completed == tcs.Task
            ? tcs.Task.Result
            : themes[new Random().Next(themes.Count)];

        _logger.LogDebug($"[{player.RoomCode}] Theme selected for player {player.Id}: {selectedTheme}");
        return selectedTheme;
    }

    public async Task<string> AskPlayerImagePromptAsync(Player player, int timeoutInSeconds)
    {
        var requestObj = new { player.RoomCode, Action = GameEngineRequests.Prompt, Payload = "" };
        var requestPayload = JsonSerializer.Serialize(requestObj);

        // Send via Service Bus and await response
        var correlationId = Guid.NewGuid().ToString();
        var tcs = new TaskCompletionSource<string>();
        _pendingRequests[correlationId] = tcs;

        var sender = _sbClient.CreateSender("gameEngineRequest");
        var requestMsg = new ServiceBusMessage(requestPayload)
        {
            SessionId = player.ConnectionId,
            CorrelationId = correlationId,
            ReplyTo = "gameEngineResponse"
        };
        await sender.SendMessageAsync(requestMsg);

        // Wait for response or timeout
        var delay = Task.Delay(TimeSpan.FromSeconds(timeoutInSeconds + 5));
        var completed = await Task.WhenAny(tcs.Task, delay);
        _pendingRequests.TryRemove(correlationId, out _);

        string selectedImagePrompt = completed == tcs.Task
            ? tcs.Task.Result
            : "";

        _logger.LogDebug($"[{player.RoomCode}] Image prompt selected for player {player.Id}: {selectedImagePrompt}");
        return selectedImagePrompt;
    }

    public async Task<PlayerAnswer> AskPlayerQuestionAsync(Player player, GameQuestion question, int timeoutInSeconds)
    {
        // Build SB request message
        var requestObj = new { player.RoomCode, Action = GameEngineRequests.Question, Payload = question };
        var requestPayload = JsonSerializer.Serialize(requestObj);

        var correlationId = Guid.NewGuid().ToString();
        var tcs = new TaskCompletionSource<string>();
        _pendingRequests[correlationId] = tcs;

        var sender = _sbClient.CreateSender("gameEngineRequest");
        var requestMsg = new ServiceBusMessage(requestPayload)
        {
            SessionId = player.ConnectionId,
            CorrelationId = correlationId,
            ReplyTo = "gameEngineResponse"
        };
        await sender.SendMessageAsync(requestMsg);
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // Wait for response or timeout
        var delay = Task.Delay(TimeSpan.FromSeconds(timeoutInSeconds + 5));
        var completed = await Task.WhenAny(tcs.Task, delay);
        _pendingRequests.TryRemove(correlationId, out _);

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

        await BroadcastGameEventAsync(player.RoomCode, GameEngineQueue.PlayerAnsweredAction, answer);
        return answer;
    }

    int CalculateBonusPoints(double elapsedTime)
    {
        if (elapsedTime <= 5)
            return 25;
        if (elapsedTime >= 25)
            return 0;
        // Linear decrease from 25 to 0 between 5 and 25 seconds
        return (int) Math.Ceiling(25 * (1 - (elapsedTime - 5) / 20));
    }
}

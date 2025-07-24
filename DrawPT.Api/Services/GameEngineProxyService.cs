using System.Text.Json;
using Azure.Messaging.ServiceBus;
using DrawPT.Api.Hubs;
using DrawPT.Common.ServiceBus;
using DrawPT.Common.Models.Game;
using Microsoft.AspNetCore.SignalR;

namespace DrawPT.Api.Services
{
    /// <summary>
    /// Listens to GameEngine messages on Service Bus and proxies them to connected SignalR clients.
    /// </summary>
    public class GameEngineProxyService : BackgroundService
    {
        private readonly ServiceBusClient _sbClient;
        private readonly IHubContext<GameHub, IGameClient> _hubContext;
        private readonly ILogger<GameEngineProxyService> _logger;
        private ServiceBusProcessor? _broadcastProcessor;
        private ServiceBusProcessor? _interactionProcessor;
        private readonly TtsService _ttsService;

        public GameEngineProxyService(
            TtsService ttsService,
            ServiceBusClient sbClient,
            IHubContext<GameHub, IGameClient> hubContext,
            ILogger<GameEngineProxyService> logger)
        {
            _ttsService = ttsService;
            _sbClient = sbClient;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _broadcastProcessor = _sbClient.CreateProcessor(
                "gameEngine",
                new ServiceBusProcessorOptions
                {
                    AutoCompleteMessages = false,
                    MaxConcurrentCalls = 64,
                    MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5)
                });

            _broadcastProcessor.ProcessMessageAsync += ProcessMessageAsync;
            _broadcastProcessor.ProcessErrorAsync += ProcessErrorAsync;

            _interactionProcessor = _sbClient.CreateProcessor(
                "gameEngineRequest",
                new ServiceBusProcessorOptions
                {
                    AutoCompleteMessages = false,
                    MaxConcurrentCalls = 64,
                    MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5)
                });

            _interactionProcessor.ProcessMessageAsync += ProcessInteractionMessageAsync;
            _interactionProcessor.ProcessErrorAsync += ProcessErrorAsync;

            _logger.LogInformation("Starting GameEngineProxyService");
            return Task.WhenAll(
                _broadcastProcessor.StartProcessingAsync(stoppingToken),
                _interactionProcessor.StartProcessingAsync(stoppingToken));
        }

        private async Task ProcessInteractionMessageAsync(ProcessMessageEventArgs args)
        {
            var connectionId = args.Message.SessionId;
            var body = args.Message.Body.ToString();
            _logger.LogInformation($"Received GameEngine interaction message for connection '{connectionId}': {body}");
            string response = string.Empty;
            try
            {
                using JsonDocument doc = JsonDocument.Parse(body);
                JsonElement root = doc.RootElement;
                string action = root.GetProperty("Action").GetString()!;
                string roomCode = root.GetProperty("RoomCode").GetString()!;
                JsonElement payload = root.GetProperty("Payload");

                var client = _hubContext.Clients.Client(connectionId);
                if (client == null)
                {
                    _logger.LogError($"Client with connection ID {connectionId} not found!");
                }
                else
                {
                    switch (action)
                    {
                        case GameEngineRequests.Theme:
                            {
                                List<string> themes = payload.Deserialize<List<string>>() ?? new List<string>();
                                using CancellationTokenSource ctsTheme = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                                await _hubContext.Clients.GroupExcept(roomCode, connectionId).ThemeSelection(themes);
                                response = await client.AskTheme(themes, ctsTheme.Token);
                                break;
                            }
                        case GameEngineRequests.Question:
                            {
                                GameQuestion question = payload.Deserialize<GameQuestion>()!;
                                using CancellationTokenSource ctsQuestion = new CancellationTokenSource(TimeSpan.FromSeconds(40));
                                var answerBase = await client.AskQuestion(question, ctsQuestion.Token);
                                response = JsonSerializer.Serialize(answerBase);
                                break;
                            }
                        default:
                            _logger.LogWarning($"Unhandled GameEngine interaction action: {action}");
                            break;
                    }
                }

                string replyTo = args.Message.ReplyTo;
                string correlationId = args.Message.CorrelationId;
                if (!string.IsNullOrEmpty(replyTo))
                {
                    ServiceBusSender sender = _sbClient.CreateSender(replyTo);
                    ServiceBusMessage replyMsg = new ServiceBusMessage(response)
                    {
                        CorrelationId = correlationId
                    };
                    await sender.SendMessageAsync(replyMsg);
                }

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing GameEngine interaction message");
            }
        }

        private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
        {
            var roomCode = args.Message.SessionId;
            var body = args.Message.Body.ToString();
            _logger.LogInformation($"Received GameEngine message for room '{roomCode}': {body}");

            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;
                var action = root.GetProperty("Action").GetString();
                var payload = root.GetProperty("Payload");

                switch (action)
                {
                    case GameEngineQueue.AnnouncerAction:
                        var accounerMsg = payload.Deserialize<string>();
                        if (accounerMsg == null)
                        {
                            _logger.LogWarning($"Received null announcer message in room: {roomCode}");
                            return;
                        }
                        await _ttsService.GenerateAudio(accounerMsg, _hubContext.Clients.Group(roomCode));
                        break;


                    case GameEngineQueue.GameStartedAction:
                        var gameState = payload.Deserialize<GameState>();
                        await _hubContext.Clients.Group(roomCode).GameStarted(gameState!);
                        break;
                    case GameEngineQueue.RoundStartedAction:
                        var round = payload.GetInt32();
                        await _hubContext.Clients.Group(roomCode).RoundStarted(round);
                        break;
                    case GameEngineQueue.PlayerThemeSelectedAction:
                        var theme = payload.GetString();
                        await _hubContext.Clients.Group(roomCode).ThemeSelected(theme!);
                        break;
                    case GameEngineQueue.PlayerScoreUpdateAction:
                        var results = payload.Deserialize<PlayerResults>();
                        await _hubContext.Clients.Group(roomCode).PlayerScoreUpdated(results!.PlayerId, results.Score);
                        break;
                    case GameEngineQueue.PlayerJoinedAction:
                        var joined = payload.Deserialize<Common.Models.Player>();
                        await _hubContext.Clients.Group(roomCode).PlayerJoined(joined!);
                        break;
                    case GameEngineQueue.PlayerLeftAction:
                        var left = payload.Deserialize<Common.Models.Player>();
                        await _hubContext.Clients.Group(roomCode).PlayerLeft(left!);
                        break;
                    case GameEngineQueue.GameResultsAction:
                        var resultsAll = payload.Deserialize<GameResults>();
                        await _hubContext.Clients.Group(roomCode).BroadcastFinalResults(resultsAll!);
                        break;
                    case GameEngineQueue.RoundResultsAction:
                        var roundResults = payload.Deserialize<RoundResults>();
                        await _hubContext.Clients.Group(roomCode).RoundResults(roundResults!);
                        break;
                    case GameEngineQueue.AssessingAnswersAction:
                        await _hubContext.Clients.Group(roomCode).WriteMessage("Assessing answers.");
                        break;
                    case GameEngineQueue.PlayerAnsweredAction:
                        var playerAnswer = payload.Deserialize<PlayerAnswer>();
                        await _hubContext.Clients.Group(roomCode).PlayerAnswered(playerAnswer);
                        break;
                    default:
                        _logger.LogWarning($"Unhandled GameEngine action: {action}");
                        break;
                }

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing GameEngine message");
            }
        }

        private Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, $"Service Bus error: {args.ErrorSource}");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_broadcastProcessor != null)
            {
                await _broadcastProcessor.StopProcessingAsync(cancellationToken);
            }

            if (_interactionProcessor != null)
            {
                await _interactionProcessor.StopProcessingAsync(cancellationToken);
            }

            await base.StopAsync(cancellationToken);
        }
    }
}

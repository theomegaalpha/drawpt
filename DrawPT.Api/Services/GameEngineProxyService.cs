using System.Text.Json;
using Azure.Messaging.ServiceBus;
using DrawPT.Api.Hubs;
using DrawPT.Common.Configuration;
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

        public GameEngineProxyService(
            ServiceBusClient sbClient,
            IHubContext<GameHub, IGameClient> hubContext,
            ILogger<GameEngineProxyService> logger)
        {
            _sbClient = sbClient;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _broadcastProcessor = _sbClient.CreateProcessor(
                "gameEngine",
                new ServiceBusProcessorOptions { AutoCompleteMessages = false });

            _broadcastProcessor.ProcessMessageAsync += ProcessMessageAsync;
            _broadcastProcessor.ProcessErrorAsync += ProcessErrorAsync;

            _interactionProcessor = _sbClient.CreateProcessor(
                "gameEngineRequest",
                new ServiceBusProcessorOptions { AutoCompleteMessages = false });

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
                        case GameEngineRequestMQ.Theme:
                            {
                                List<string> themes = payload.Deserialize<List<string>>() ?? new List<string>();
                                using CancellationTokenSource ctsTheme = new CancellationTokenSource(TimeSpan.FromSeconds(35));
                                response = await client.AskTheme(themes, ctsTheme.Token);
                                break;
                            }
                        case GameEngineRequestMQ.Question:
                            {
                                GameQuestion question = payload.Deserialize<GameQuestion>()!;
                                using CancellationTokenSource ctsQuestion = new CancellationTokenSource(TimeSpan.FromSeconds(35));
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

        int CalculateBonusPoints(double elapsedTime)
        {
            if (elapsedTime <= 3)
                return 5;
            if (elapsedTime >= 15)
                return 0;
            // Linear decrease from 5 to 0 between 3 and 15 seconds
            return (int) Math.Ceiling(5 * (1 - (elapsedTime - 3) / 12));
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
                    case GameEngineBroadcastMQ.GameStartedAction:
                        var gameState = payload.Deserialize<GameState>();
                        await _hubContext.Clients.Group(roomCode).GameStarted(gameState!);
                        break;
                    case GameEngineBroadcastMQ.RoundStartedAction:
                        var round = payload.GetInt32();
                        await _hubContext.Clients.Group(roomCode).RoundStarted(round);
                        break;
                    case GameEngineBroadcastMQ.PlayerThemeSelectedAction:
                        var theme = payload.GetString();
                        await _hubContext.Clients.Group(roomCode).ThemeSelected(theme!);
                        break;
                    case GameEngineBroadcastMQ.PlayerScoreUpdateAction:
                        var results = payload.Deserialize<PlayerResults>();
                        await _hubContext.Clients.Group(roomCode).PlayerScoreUpdated(results!.PlayerId, results.Score);
                        break;
                    case GameEngineBroadcastMQ.PlayerLeftAction:
                        var left = payload.Deserialize<Common.Models.Player>();
                        await _hubContext.Clients.Group(roomCode).PlayerLeft(left!);
                        break;
                    case GameEngineBroadcastMQ.GameResultsAction:
                        var resultsAll = payload.Deserialize<GameResults>();
                        await _hubContext.Clients.Group(roomCode).BroadcastFinalResults(resultsAll!);
                        break;
                    case GameEngineBroadcastMQ.RoundResultsAction:
                        var roundResults = payload.Deserialize<RoundResults>();
                        await _hubContext.Clients.Group(roomCode).RoundResults(roundResults!);
                        break;
                    case GameEngineBroadcastMQ.AssessingAnswersAction:
                        await _hubContext.Clients.Group(roomCode).WriteMessage("Assessing answers.");
                        break;
                    case GameEngineBroadcastMQ.PlayerAnsweredAction:
                        var playerAnswer = payload.Deserialize<PlayerAnswer>();
                        if (playerAnswer != null)
                        {
                            await _hubContext.Clients.Group(roomCode).WriteMessage($"{playerAnswer.Username} answered!");
                        }
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
            await base.StopAsync(cancellationToken);
        }
    }
}

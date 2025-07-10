using Azure.Messaging.ServiceBus;
using DrawPT.GameEngine.Interfaces;
using DrawPT.Common.Models.Game;
using System.Text.Json;

namespace DrawPT.GameEngine.BackgroundWorkers;

public class GameEventListener : BackgroundService
{
    private readonly ILogger<GameEventListener> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ServiceBusClient _serviceBusClient;

    public GameEventListener(
        ILogger<GameEventListener> logger,
        ServiceBusClient serviceBusClient,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _serviceBusClient = serviceBusClient;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Configure Azure Service Bus processor for 'apiGlobal' queue
        var processor = _serviceBusClient.CreateProcessor("apiGlobal");

        // Handle incoming Service Bus messages
        processor.ProcessMessageAsync += async args =>
        {
            var body = args.Message.Body.ToString();
            _logger.LogInformation($"Received Service Bus message: {body}");
            var gameState = JsonSerializer.Deserialize<GameState>(body);
            _logger.LogInformation($"Game start event for room: {gameState.RoomCode}");
            // Resolve scoped IGameSession per message
            _ = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var gameEngine = scope.ServiceProvider.GetRequiredService<IGameSession>();
                await gameEngine.PlayGameAsync(gameState.RoomCode);
            });
            await args.CompleteMessageAsync(args.Message);
        };
        processor.ProcessErrorAsync += args =>
        {
            _logger.LogError(args.Exception, "Error processing Service Bus message");
            return Task.CompletedTask;
        };

        // Stop processing on cancellation
        stoppingToken.Register(async () => await processor.StopProcessingAsync());
        await processor.StartProcessingAsync();

        _logger.LogInformation("Started consuming from game queue");

        return;
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}

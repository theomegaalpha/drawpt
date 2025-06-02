using DrawPT.Common.Interfaces;
using DrawPT.GameEngine.BackgroundWorkers;
using DrawPT.GameEngine.Interfaces;
using DrawPT.GameEngine.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRabbitMQClient(connectionName: "messaging");
builder.AddAzureBlobClient(connectionName: "storage");
builder.AddRedisDistributedCache(connectionName: "cache");

var services = builder.Services;
services.AddTransient<IAIService, AIService>();
services.AddTransient<IGameFlowController, GameFlowController>();
services.AddHostedService<GameEventListener>();
services.AddTransient<IStorageService, StorageService>();
services.AddTransient<IPlayerManager, PlayerManager>();
services.AddTransient<IRoundOrchestrator, RoundOrchestrator>();

var app = builder.Build();
app.UseHttpsRedirection();
app.Run();

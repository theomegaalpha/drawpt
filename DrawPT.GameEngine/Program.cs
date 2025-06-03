using DrawPT.Common.Interfaces;
using DrawPT.Common.Services;
using DrawPT.GameEngine;
using DrawPT.GameEngine.BackgroundWorkers;
using DrawPT.GameEngine.Interfaces;
using DrawPT.GameEngine.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRabbitMQClient(connectionName: "messaging");
builder.AddAzureBlobClient(connectionName: "storage");
builder.AddRedisDistributedCache(connectionName: "cache");

var services = builder.Services;
services.AddHostedService<GameEventListener>();
services.AddTransient<IAIService, AIService>();
services.AddTransient<ICacheService, CacheService>();
services.AddTransient<IStorageService, StorageService>();
services.AddTransient<IPlayerManager, PlayerManager>();
services.AddTransient<IGameCommunicationService, GameCommunicationService>();
services.AddTransient<IGameEngine, GameEngine>();

var app = builder.Build();
app.UseHttpsRedirection();
app.Run();

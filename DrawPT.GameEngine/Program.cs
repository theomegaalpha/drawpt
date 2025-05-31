using DrawPT.GameEngine;
using DrawPT.GameEngine.Interfaces;
using DrawPT.GameEngine.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRabbitMQClient(connectionName: "messaging");
builder.AddAzureBlobClient(connectionName: "storage");
builder.AddRedisDistributedCache(connectionName: "cache");
builder.Services.AddHostedService<GameOrchestrator>();

var services = builder.Services;
services.AddScoped<IAIService, AIService>();
//services.AddScoped<IStorageService, StorageService>();
//services.AddScoped<IPlayerManager, PlayerManager>();
//services.AddScoped<IRoundManager, RoundManager>();
//services.AddScoped<IQuestionManager, QuestionManager>();
//services.AddScoped<IGameEngine, GameEngine>();

var app = builder.Build();
app.UseHttpsRedirection();
app.Run();

using DrawPT.Common.Interfaces;
using DrawPT.Common.Services;
using DrawPT.Data.Repositories;
using DrawPT.GameEngine;
using DrawPT.GameEngine.BackgroundWorkers;
using DrawPT.GameEngine.LocalCache;
using DrawPT.GameEngine.Interfaces;
using DrawPT.GameEngine.Services;
using DrawPT.Common.Interfaces.Game;

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
services.AddTransient<IThemeService, ThemeService>();
services.AddTransient<IGameCommunicationService, GameCommunicationService>();
services.AddTransient<IGameEngine, GameEngine>();
services.AddTransient<ReferenceRepository>();

builder.Services.AddSingleton<ThemeCache>();
builder.AddSqlServerDbContext<ReferenceDbContext>(connectionName: "database");


var app = builder.Build();

// Build ReferenceCache at startup
using (var scope = app.Services.CreateScope())
{
    var referenceRepository = scope.ServiceProvider.GetRequiredService<ReferenceRepository>();
    var referenceCache = app.Services.GetRequiredService<ThemeCache>();
    referenceCache.BuildCache(referenceRepository);
}

app.UseHttpsRedirection();
app.Run();

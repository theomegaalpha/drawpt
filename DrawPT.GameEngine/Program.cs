using DrawPT.Common.Interfaces;
using DrawPT.Common.Services;
using DrawPT.Data.Repositories;
using DrawPT.GameEngine;
using DrawPT.GameEngine.BackgroundWorkers;
using DrawPT.GameEngine.LocalCache;
using DrawPT.GameEngine.Interfaces;
using DrawPT.GameEngine.Services;
using DrawPT.Common.Interfaces.Game;
using DrawPT.Common.Services.AI;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
       .SetBasePath(AppContext.BaseDirectory)
       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
       .AddEnvironmentVariables()
       .Build();

builder.AddServiceDefaults();
builder.Services.AddSingleton<IConfiguration>(configuration);

builder.AddAzureServiceBusClient(connectionName: "service-bus");
builder.AddAzureBlobClient(connectionName: "blobs");
builder.AddRedisDistributedCache(connectionName: "cache");
builder.AddAzureOpenAIClient(connectionName: "openai");

var services = builder.Services;
services.AddHostedService<GameEventListener>();
services.AddTransient<GeminiImageGenerator>();
services.AddTransient<FreepikFastService>();
services.AddTransient<IAIService, AIService>();
services.AddTransient<IGameAnnouncerService, GameAnnouncerService>();
services.AddTransient<IAssessmentService, AssessmentService>();
services.AddTransient<ICacheService, CacheService>();
services.AddTransient<IStorageService, StorageService>();
services.AddTransient<IPlayerManager, PlayerManager>();
services.AddTransient<IThemeService, ThemeService>();
services.AddTransient<IQuestionService, QuestionService>();
services.AddTransient<IGameCommunicationService, GameCommunicationService>();
services.AddTransient<IGameStateService, GameStateService>();
services.AddTransient<IGameSession, GameSession>();
services.AddTransient<ReferenceRepository>();
services.AddTransient<GameEntitiesRepository>();
// Register Supabase client for PlayerService dependency
builder.Services.AddTransient<Supabase.Client>(sp =>
{
    var url = builder.Configuration["SupabaseUrl"];
    var secretKey = builder.Configuration["SupabaseApiKey"];
    var options = new Supabase.SupabaseOptions { AutoConnectRealtime = true };
    var client = new Supabase.Client(url, secretKey, options);
    client.InitializeAsync().Wait();
    return client;
});
// Register PlayerService so CacheService can resolve it
services.AddTransient<PlayerService>();

builder.Services.AddSingleton<ThemeCache>();
builder.AddSqlServerDbContext<ReferenceDbContext>(connectionName: "database");
builder.AddSqlServerDbContext<GameEntitiesDbContext>(connectionName: "database");


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

using DrawPT.Api.AI;
using DrawPT.Api.Cache;
using DrawPT.Api.Engine;
using DrawPT.Api.Hubs;
using DrawPT.Data.Models;
using DrawPT.Api.Services;
using DrawPT.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.SignalR;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TODO: Add Redis distributed cache
builder.Services.AddDistributedMemoryCache();

builder.AddSqlServerClient(connectionName: "database");
builder.AddSqlServerDbContext<ReferenceDbContext>(connectionName: "database");
builder.AddSqlServerDbContext<ImageDbContext>(connectionName: "database");
builder.AddAzureOpenAIClient(connectionName: "openai");
builder.AddAzureBlobClient("blobs");

builder.Services.AddTransient<StorageService>();
builder.Services.AddTransient<ImageRepository>();
builder.Services.AddTransient<ReferenceRepository>();
builder.Services.AddTransient<AIClient>();
builder.Services.AddTransient<GeminiImageGenerator>();
builder.Services.AddTransient<RandomService>();
builder.Services.AddSingleton<CacheService>();
builder.Services.AddSingleton<ReferenceCache>();
builder.Services.AddSingleton<GameCollection>();
builder.Services.AddSingleton<IGameFactory, GameFactory>();
builder.Services.AddOptions<GameOptions>()
                .BindConfiguration("Game");

builder.Services.AddScoped<Game>();

// Add SignalR with CORS
builder.Services.AddSignalR()
    .AddAzureSignalR(builder.Configuration.GetConnectionString("signalr"));

// Add health checks
builder.Services.AddHealthChecks();

// Add Application Insights Telemetry
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

// Build ReferenceCache at startup
using (var scope = app.Services.CreateScope())
{
    var referenceRepository = scope.ServiceProvider.GetRequiredService<ReferenceRepository>();
    var referenceCache = app.Services.GetRequiredService<ReferenceCache>();
    referenceCache.BuildCache(referenceRepository);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Map SignalR Hubs
app.MapHub<GameHub>("/gamehub");

// Use health checks
app.UseHealthChecks("/health");

app.Run();

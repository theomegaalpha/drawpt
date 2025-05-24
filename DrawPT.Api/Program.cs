using DrawPT.Api.AI;
using DrawPT.Api.Cache;
using DrawPT.Api.Engine;
using DrawPT.Api.Hubs;
using DrawPT.Api.Models;
using DrawPT.Api.Repositories;
using DrawPT.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
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

builder.Services.AddDbContext<ImageDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetValue<string>("SqlConnectionString")),
    contextLifetime: ServiceLifetime.Transient,
    optionsLifetime: ServiceLifetime.Transient);
builder.Services.AddDbContext<ReferenceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetValue<string>("SqlConnectionString")),
    ServiceLifetime.Singleton);

builder.Services.AddTransient<StorageService>();
builder.Services.AddTransient<ImageRepository>();
builder.Services.AddTransient<ReferenceRepository>();
builder.Services.AddTransient<AIClient>();
builder.Services.AddTransient<CacheService>();
builder.Services.AddTransient<RandomService>();
builder.Services.AddSingleton<ReferenceCache>();
builder.Services.AddSingleton<GameCollection>();
builder.Services.AddOptions<GameOptions>()
                .BindConfiguration("Game");

builder.Services.AddTransient<Game>();

// Add SignalR
builder.Services.AddSignalR();

// Add health checks
builder.Services.AddHealthChecks();

// Add Application Insights Telemetry
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// get Reference Cache and build it
var referenceCache = app.Services.GetRequiredService<ReferenceCache>();
referenceCache.BuildCache();

// Setup CORS policy
var allowedOrigins = builder.Configuration.GetValue<string>("CORS:AllowedOrigins");
// add logging for allowedOrigins
app.Logger.LogInformation($"Allowed Origins: {allowedOrigins}");
if (!string.IsNullOrWhiteSpace(allowedOrigins))
{
    var allowedOriginsTokens = allowedOrigins.Split(",", StringSplitOptions.RemoveEmptyEntries);
    app.UseCors(x => x.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins(allowedOriginsTokens)
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(2520))
                    .WithExposedHeaders("WWW-Authenticate"));
}
else
{
    app.UseCors(x => x.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin());
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Map SignalR Hubs
app.MapHub<GameHub>("/gameHub");

// Use health checks
app.UseHealthChecks("/health");

app.Run();

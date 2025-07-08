using DrawPT.Api.Cache;
using DrawPT.Api.Hubs;
using DrawPT.Api.Services;
using DrawPT.Common.Interfaces;
using DrawPT.Common.Services;
using DrawPT.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DrawPT.Common.Services.AI;

var builder = WebApplication.CreateBuilder(args);

var bytes = Encoding.UTF8.GetBytes(builder.Configuration["AuthenticationSecretKey"]!);
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(bytes),
        ValidAudience = builder.Configuration["Authentication:ValidAudience"],
        ValidIssuer = builder.Configuration["AuthenticationValidIssuer"],
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Headers.Authorization.FirstOrDefault() ?? "";
            accessToken = accessToken.Replace("Bearer ", "");

            // If the request is for our hub...
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/gamehub")))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TODO: Add Redis distributed cache
builder.Services.AddDistributedMemoryCache();

// Register Text-to-Speech service for streaming audio
builder.Services.AddTransient<TtsService>();

builder.AddAzureServiceBusClient(connectionName: "service-bus");
builder.AddSqlServerClient(connectionName: "database");
builder.AddSqlServerDbContext<ReferenceDbContext>(connectionName: "database");
builder.AddSqlServerDbContext<DailiesDbContext>(connectionName: "database");
builder.AddRedisDistributedCache(connectionName: "cache");
builder.AddAzureOpenAIClient(connectionName: "openai");
builder.AddAzureBlobClient("blobs");
builder.Services.AddTransient<Supabase.Client>(sp =>
{
    var url = builder.Configuration["SupabaseUrl"];
    var secretKey = builder.Configuration["SupabaseApiKey"];
    var options = new Supabase.SupabaseOptions
    {
        AutoConnectRealtime = true
    };
    var client = new Supabase.Client(url, secretKey, options);
    client.InitializeAsync().Wait();
    return client;
});
builder.Services.AddTransient<PlayerService>();


builder.Services.AddTransient<StorageService>();
builder.Services.AddTransient<DailiesRepository>();
builder.Services.AddTransient<ReferenceRepository>();
builder.Services.AddTransient<RandomService>();
builder.Services.AddTransient<CacheService>();
builder.Services.AddTransient<ICacheService, CacheService>();
builder.Services.AddSingleton<ReferenceCache>();

builder.Services.AddTransient<IStorageService, StorageService>();
builder.Services.AddTransient<FreepikMysticService>();
builder.Services.AddTransient<FreepikFastService>();
builder.Services.AddTransient<DailyAIService>();

// Add SignalR with CORS
builder.Services.AddSignalR()
    .AddAzureSignalR(builder.Configuration.GetConnectionString("signalr"));

// Add health checks
builder.Services.AddHealthChecks();

// Add Application Insights Telemetry
builder.Services.AddApplicationInsightsTelemetry();

// Add authorization policy for admin users
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("user_role", "admin"));
});

// Add GameEngineProxyService background listener
builder.Services.AddHostedService<GameEngineProxyService>();

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

// Ensure authentication runs before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map SignalR Hubs
app.MapHub<GameHub>("/gamehub");
app.MapHub<NotificationHub>("/notificationhub");

// Use health checks
app.UseHealthChecks("/health");

app.Run();

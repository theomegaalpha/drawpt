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

var builder = WebApplication.CreateBuilder(args);

var bytes = Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretKey"]!);
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(bytes),
        ValidAudience = builder.Configuration["Authentication:ValidAudience"],
        ValidIssuer = builder.Configuration["Authentication:ValidIssuer"],
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

builder.AddSqlServerClient(connectionName: "database");
builder.AddSqlServerDbContext<ReferenceDbContext>(connectionName: "database");
builder.AddSqlServerDbContext<ImageDbContext>(connectionName: "database");
builder.AddRedisDistributedCache(connectionName: "cache");
builder.AddRabbitMQClient(connectionName: "messaging");
builder.AddAzureOpenAIClient(connectionName: "openai");
builder.AddAzureBlobClient("blobs");
builder.Services.AddTransient<Supabase.Client>(sp =>
{
    var url = builder.Configuration["Supabase:Url"];
    var secretKey = builder.Configuration["Supabase:ApiKey"];
    var options = new Supabase.SupabaseOptions
    {
        AutoConnectRealtime = true
    };
    var client = new Supabase.Client(url, secretKey, options);
    client.InitializeAsync().Wait();
    return client;
});
builder.Services.AddTransient<ProfileService>();


builder.Services.AddTransient<StorageService>();
builder.Services.AddTransient<ImageRepository>();
builder.Services.AddTransient<ReferenceRepository>();
builder.Services.AddTransient<RandomService>();
builder.Services.AddTransient<CacheService>();
builder.Services.AddTransient<ICacheService, CacheService>();
builder.Services.AddSingleton<ReferenceCache>();

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

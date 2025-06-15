using DrawPT.Common.Interfaces;
using DrawPT.Common.Services;
using DrawPT.Common.Services.AI;
using DrawPT.Data.Repositories;
using DrawPT.ScheduledService;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Add the new scheduled service
builder.AddAzureOpenAIClient(connectionName: "openai");
builder.AddAzureBlobClient(connectionName: "blobs");
builder.Services.AddTransient<IStorageService, StorageService>();
builder.Services.AddTransient<FreepikMysticService>();
builder.Services.AddTransient<DailyAIService>();
builder.Services.AddScoped<DailiesRepository>(); // Changed from AddTransient to AddScoped

builder.AddSqlServerDbContext<DailiesDbContext>(connectionName: "database");
builder.Services.AddHostedService<DailyMidnightTaskService>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.Run();

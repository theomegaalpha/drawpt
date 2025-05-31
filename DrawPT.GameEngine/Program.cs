using DrawPT.GameEngine.Interfaces;
using DrawPT.GameEngine.Managers;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRabbitMQClient(connectionName: "messaging");

var services = builder.Services;
services.AddScoped<IPlayerManager, PlayerManager>();

var app = builder.Build();
app.UseHttpsRedirection();
app.Run();

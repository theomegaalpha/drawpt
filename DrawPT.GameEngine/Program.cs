using DrawPT.GameEngine.Interfaces;
using DrawPT.GameEngine.Managers;
using DrawPT.GameEngine;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRabbitMQClient(connectionName: "messaging");

var services = builder.Services;
services.AddScoped<IPlayerManager, PlayerManager>();
services.AddScoped<IRoundManager, RoundManager>();
services.AddScoped<IQuestionManager, QuestionManager>();
services.AddScoped<IGameEngine, GameEngine>();

var app = builder.Build();
app.UseHttpsRedirection();
app.Run();

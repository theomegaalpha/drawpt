var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

services.AddSingleton<IGameEventBus, GameEventBus>();
services.AddScoped<IGameEngine, GameEngine>();
services.AddScoped<IPlayerManager, PlayerManager>();
services.AddScoped<IRoundManager, RoundManager>();
services.AddScoped<IQuestionManager, QuestionManager>();

var app = builder.Build();
app.UseHttpsRedirection();
app.Run();

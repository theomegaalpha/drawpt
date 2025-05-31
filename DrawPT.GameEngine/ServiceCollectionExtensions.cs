using DrawPT.GameEngine.Infrastructure;
using DrawPT.GameEngine.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DrawPT.GameEngine;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameEngine(this IServiceCollection services, string redisConnectionString)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp => 
            ConnectionMultiplexer.Connect(redisConnectionString));
            
        services.AddSingleton<IGameStateManager, RedisGameStateManager>();
        services.AddScoped<IGameEngine, GameEngine>();
        
        return services;
    }
} 
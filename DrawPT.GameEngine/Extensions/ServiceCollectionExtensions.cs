using DrawPT.GameEngine.Configuration;
using DrawPT.GameEngine.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DrawPT.GameEngine.Extensions
{
    /// <summary>
    /// Extension methods for IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds RabbitMQ and game event services to the service collection
        /// </summary>
        public static IServiceCollection AddGameEngineServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configure RabbitMQ settings
            services.Configure<RabbitMQSettings>(
                configuration.GetSection("RabbitMQ"));

            // Register services
            services.AddSingleton<RabbitMQService>();
            services.AddSingleton<GameEventService>();

            return services;
        }
    }
} 
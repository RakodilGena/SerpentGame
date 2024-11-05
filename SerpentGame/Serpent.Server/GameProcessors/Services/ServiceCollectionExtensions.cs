using Serpent.Server.GameProcessors.Services.Factories;
using Serpent.Server.GameProcessors.Services.Factories.Impl;

namespace Serpent.Server.GameProcessors.Services;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameProcessorServices(this IServiceCollection services)
    {
        return services
            .AddTransient<ISnakeFactory, SnakeFactory>()
            .AddTransient<IConsumablesFactory, ConsumablesFactory>();
    }
}
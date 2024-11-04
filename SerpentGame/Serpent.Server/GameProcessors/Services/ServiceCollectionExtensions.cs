using Serpent.Server.GameProcessors.Services.Impl;

namespace Serpent.Server.GameProcessors.Services;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameProcessorServices(this IServiceCollection services)
    {
        return services.AddScoped<ISnakeFactory, SnakeFactory>();
    }
}
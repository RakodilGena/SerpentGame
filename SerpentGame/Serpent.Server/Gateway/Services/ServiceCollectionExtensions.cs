using Serpent.Server.Gateway.Services.Impl;

namespace Serpent.Server.Gateway.Services;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGatewayServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IStartGameService, StartGameService>()
            .AddScoped<IFinishGameService, FinishGameService>()
            .AddScoped<IUserRecordsService, UserRecordsService>();
    }
}
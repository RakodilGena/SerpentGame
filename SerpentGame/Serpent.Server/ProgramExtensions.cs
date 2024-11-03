using System.Diagnostics;

namespace Serpent.Server;

internal static class ProgramExtensions
{
    public static IServiceCollection AddCorsDefaultPolicy(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var allowOrigins =
            configuration.GetSection("AllowOrigins").Get<string[]>()
            ?? [];
        
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder.AllowAnyMethod().AllowAnyHeader();
                    if (allowOrigins.Length > 0)
                        builder.WithOrigins(allowOrigins);
                    else
                        builder.SetIsOriginAllowed(_ => true);
                    builder.AllowCredentials();
                });
            Trace.TraceInformation(string.Join(",", allowOrigins));
        });

        return services;
    }
}
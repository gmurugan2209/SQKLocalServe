using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SQKLocalServe.Common.Logging;

public static class Extensions
{
    public static IServiceCollection AddApplicationLogging(this IServiceCollection services)
    {
        services.AddSingleton<ILogManager, NLogManager>();
        return services;
    }

    public static IApplicationBuilder UseApplicationLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<LoggingMiddleware>();
        return app;
    }
}

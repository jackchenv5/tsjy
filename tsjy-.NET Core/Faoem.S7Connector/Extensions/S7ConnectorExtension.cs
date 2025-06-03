using Faoem.S7Connector.Services.S7Connector;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Faoem.S7Connector.Extensions;

public static class S7ConnectorExtension
{
    public static void AddS7Connector(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<S7ConnectorService>();
    }

    public static WebApplication ConfigureS7Connector(this WebApplication app)
    {
        return app;
    }
}
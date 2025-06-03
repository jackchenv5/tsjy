using Faoem.OpcUaConnector.Services.OpcUaConnector;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Faoem.OpcUaConnector.Extensions;

public static class OpcUaConnectorExtension
{
    public static void AddOpcUaConnector(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<OpcUaConnectorService>();
    }

    public static WebApplication ConfigureOpcUaConnector(this WebApplication app)
    {
        return app;
    }
}
using Faoem.ModbusTcpConnector.Services.ModbusTcpConnector;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Faoem.ModbusTcpConnector.Extensions;

public static class ModbusTcpConnectorExtension
{
    public static void AddModbusTcpConnector(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<ModbusTcpConnectorService>();
    }

    public static WebApplication ConfigureModbusTcpConnector(this WebApplication app)
    {
        return app;
    }
}
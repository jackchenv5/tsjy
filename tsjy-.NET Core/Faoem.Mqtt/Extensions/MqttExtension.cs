using Faoem.Mqtt.Services.MqttClient;
using Faoem.Mqtt.Services.MqttClientHosted;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MqttClientOptions = Faoem.Mqtt.Options.MqttClientOptions;

namespace Faoem.Mqtt.Extensions;

public static class MqttExtension
{
    public static void AddMqtt(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMqttClient(configuration);
        services.AddSingleton<IMqttClientService, MqttClientService>();
    }

    public static WebApplication ConfigureMqtt(this WebApplication app)
    {
        app.AddDefaultMenu();

        return app;
    }

    private static void AddMqttClient(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection("MqttClient").Get<MqttClientOptions>()
                      ?? new MqttClientOptions();

        var mqttClientOptionsBuilder = new MqttClientOptionsBuilder()
            .WithCredentials(options.Username, options.Password)
            .WithTcpServer(options.Host, options.Port)
            .WithClientId($"{options.ClientIdPrefix}{Guid.NewGuid().ToString("N")[..8]}")
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(options.KeepAlivePeriod));

        if (options.UseTls)
        {
            mqttClientOptionsBuilder.WithTlsOptions(builder =>
            {
                builder.UseTls();
                // 不验证证书合法性（支持自签名证书）
                builder.WithCertificateValidationHandler(_ => true);
            });
        }

        var mqttClientOptions = mqttClientOptionsBuilder.Build();
        var managedMqttClientOptions = new ManagedMqttClientOptionsBuilder()
            .WithClientOptions(mqttClientOptions)
            .WithAutoReconnectDelay(TimeSpan.FromSeconds(options.AutoReconnectDelay))
            .Build();
        var managedMqttClient = new MqttFactory().CreateManagedMqttClient();

        services.AddSingleton(managedMqttClient);
        services.AddSingleton(managedMqttClientOptions);
        services.AddHostedService<MqttClientHostedService>();
    }
}
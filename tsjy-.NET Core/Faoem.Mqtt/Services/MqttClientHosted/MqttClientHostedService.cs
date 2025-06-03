using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;

namespace Faoem.Mqtt.Services.MqttClientHosted;

public class MqttClientHostedService : IHostedService, IDisposable
{
    private readonly IManagedMqttClient _mqttClient;
    private readonly ManagedMqttClientOptions _options;
    private readonly ILogger<MqttClientHostedService> _logger;

    public MqttClientHostedService(IManagedMqttClient mqttClient, ManagedMqttClientOptions options,
        ILogger<MqttClientHostedService> logger)
    {
        _mqttClient = mqttClient;
        _options = options;
        _logger = logger;

        _mqttClient.ConnectedAsync += MqttClientOnConnectedAsync;
        _mqttClient.DisconnectedAsync += MqttClientOnDisconnectedAsync;
    }

    private async Task MqttClientOnConnectedAsync(MqttClientConnectedEventArgs arg)
    {
        _logger.LogInformation("Mqtt client connected.");

        await Task.CompletedTask;
    }

    private async Task MqttClientOnDisconnectedAsync(MqttClientDisconnectedEventArgs arg)
    {
        _logger.LogInformation("Mqtt client disconnected.");

        await Task.CompletedTask;
    }

    async Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        await _mqttClient.StartAsync(_options);
    }

    async Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        await _mqttClient.StopAsync();
    }

    public void Dispose()
    {
        _mqttClient.Dispose();
    }
}
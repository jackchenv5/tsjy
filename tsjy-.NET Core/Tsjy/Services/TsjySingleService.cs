using MQTTnet.Extensions.ManagedClient;
using System.Text.Json;
using MQTTnet.Protocol;

namespace Tsjy.Services;

public class TsjySingleService
{
    private readonly IManagedMqttClient _mqttClient;

    public TsjySingleService(IManagedMqttClient mqttClient)
    {
        _mqttClient = mqttClient;
    }

    public async Task RequestAllVariableAsync()
    {
        const string topic = "ie/c/j/simatic/v1/updaterequest";
        var payloadObj = new { Path = string.Empty };
        var payload = JsonSerializer.Serialize(payloadObj);

        await _mqttClient.EnqueueAsync(topic, payload, MqttQualityOfServiceLevel.AtLeastOnce);
    }
}
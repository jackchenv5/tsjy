using Faoem.Mqtt.Dtos;
using MQTTnet.Protocol;

namespace Faoem.Mqtt.Services.MqttClient;

public interface IMqttClientService
{
    public Task<MqttClientStatusDto> GetMqttClientStatusAsync();

    public Task SubscribeAsync(string topic, MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtMostOnce);
    public Task UnsubscribeAsyncAsync(string topic);
    public Task<List<string>> GetSubscribedTopicsAsync();
    public Task<List<MqttMessageDto>> GetMessagesAsync(string topic);
}
using System.Text;
using Faoem.Mqtt.Dtos;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;

namespace Faoem.Mqtt.Services.MqttClient;

internal class MqttClientService : IMqttClientService
{
    private readonly IManagedMqttClient _mqttClient;
    private readonly Dictionary<string, Queue<MqttMessageDto>> _messages = new();
    private readonly HashSet<string> _topics = [];

    private const int MaxQueueSize = 100;

    public MqttClientService(IManagedMqttClient mqttClient)
    {
        _mqttClient = mqttClient;

        _mqttClient.ApplicationMessageReceivedAsync += MqttClientOnApplicationMessageReceivedAsync;
    }

    private async Task MqttClientOnApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        var topic = arg.ApplicationMessage.Topic;
        var payload = Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment);

        if (!_messages.TryGetValue(topic, out var value))
        {
            value = new Queue<MqttMessageDto>();
            _messages.Add(topic, value);
        }

        var queue = value;
        if (queue.Count == MaxQueueSize)
        {
            queue.Dequeue();
        }

        queue.Enqueue(new MqttMessageDto
        {
            Topic = topic,
            Payload = payload,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Retained = arg.ApplicationMessage.Retain,
            Qos = arg.ApplicationMessage.QualityOfServiceLevel
        });

        await Task.CompletedTask;
    }

    public async Task SubscribeAsync(string topic,
        MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtMostOnce)
    {
        await _mqttClient.SubscribeAsync(topic, qos);
        _topics.Add(topic);
    }

    public async Task UnsubscribeAsyncAsync(string topic)
    {
        await _mqttClient.UnsubscribeAsync(topic);
        _topics.Remove(topic);
        await RemoveMessagesAsync(topic);
    }

    /// <summary>
    /// 获取所以订阅的主题，订阅的主题可能包含通配符（#，+）。例如：ie/#
    /// </summary>
    /// <returns></returns>
    public async Task<List<string>> GetSubscribedTopicsAsync()
    {
        return await Task.FromResult(_topics.ToList());
    }

    /// <summary>
    /// 获取指定订阅主题相关的消息
    /// </summary>
    /// <param name="topic">订阅的主题，可能包含通配符（*，+）</param>
    /// <returns></returns>
    public Task<List<MqttMessageDto>> GetMessagesAsync(string topic)
    {
        var queues = _messages.Keys
            .Where(k => MqttTopicFilterComparer.Compare(k, topic) == MqttTopicFilterCompareResult.IsMatch)
            .Select(k => _messages[k]);

        var messages = new List<MqttMessageDto>();
        foreach (var queue in queues)
        {
            messages.AddRange(queue.ToList());
        }

        return Task.FromResult(messages.OrderBy(m => m.Timestamp).ToList());
    }

    /// <summary>
    /// 获取 mqtt 客户端的状态。
    /// </summary>
    /// <returns></returns>
    public async Task<MqttClientStatusDto> GetMqttClientStatusAsync()
    {
        return await Task.FromResult(new MqttClientStatusDto
        {
            ClientId = _mqttClient.Options.ClientOptions.ClientId,
            IsConnected = _mqttClient.IsConnected
        });
    }

    /// <summary>
    /// 移除指定订阅主题相关的消息
    /// </summary>
    /// <param name="topic">订阅的主题，可能包含通配符（*，+）</param>
    private Task RemoveMessagesAsync(string topic)
    {
        var fullTopics = _messages.Keys
            .Where(k => MqttTopicFilterComparer.Compare(k, topic) == MqttTopicFilterCompareResult.IsMatch)
            .ToList();

        foreach (var fullTopic in fullTopics)
        {
            _messages.Remove(fullTopic);
        }

        return Task.CompletedTask;
    }
}
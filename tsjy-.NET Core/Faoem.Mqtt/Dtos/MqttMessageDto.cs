using MQTTnet.Protocol;

namespace Faoem.Mqtt.Dtos;

public class MqttMessageDto
{
    public string Topic { get; set; } = null!;
    public string? Payload { get; set; }
    public long Timestamp { get; set; }
    public bool Retained { get; set; }
    public MqttQualityOfServiceLevel Qos { get; set; }
}
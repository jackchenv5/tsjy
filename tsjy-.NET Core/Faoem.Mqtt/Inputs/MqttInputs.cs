using MQTTnet.Protocol;

namespace Faoem.Mqtt.Inputs;

public class MqttInputs
{
    public string? Topic { get; set; }
    public MqttQualityOfServiceLevel? Qos { get; set; }
}
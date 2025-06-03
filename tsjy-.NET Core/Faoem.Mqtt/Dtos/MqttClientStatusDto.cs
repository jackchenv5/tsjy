namespace Faoem.Mqtt.Dtos;

public class MqttClientStatusDto
{
    public string ClientId { get; set; } = "";
    public bool IsConnected { get; set; }
}
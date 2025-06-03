namespace Faoem.Mqtt.Options;

public class MqttClientOptions
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 1883;
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string ClientIdPrefix { get; set; } = "faoem-app-";
    public bool UseTls { get; set; }
    public int KeepAlivePeriod { get; set; } = 10;
    public int AutoReconnectDelay { get; set; } = 10;
}
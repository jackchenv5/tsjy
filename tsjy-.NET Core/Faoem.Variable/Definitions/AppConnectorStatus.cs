namespace Faoem.Variable.Definitions;

public class AppConnectorStatus(
    string connectorInstance,
    long seq,
    DateTimeOffset ts,
    string connectorStatus,
    List<AppConnectionStatus> connectionStatus)
{
    public string ConnectorInstance { get; set; } = connectorInstance;
    public long Seq { get; set; } = seq;
    public DateTimeOffset Ts { get; set; } = ts;
    public string ConnectorStatus { get; set; } = connectorStatus;
    public List<AppConnectionStatus> ConnectionStatus { get; set; } = connectionStatus;
}
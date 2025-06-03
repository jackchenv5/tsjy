namespace Faoem.ModbusTcpConnector.Definitions;

public class Status
{
    public long Seq { get; set; }
    public DateTimeOffset Ts { get; set; }
    public ConnectorStatus Connector { get; set; } = null!;
    public List<ConnectionStatus> Connections { get; set; } = null!;
}
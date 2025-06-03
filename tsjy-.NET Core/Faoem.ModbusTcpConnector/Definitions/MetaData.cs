namespace Faoem.ModbusTcpConnector.Definitions;

public class MetaData
{
    /// <summary>
    /// e.g. "Modbus TCP Connector (Siemens AG)"
    /// </summary>
    public string ApplicationName { get; set; } = "Modbus TCP Connector (Siemens AG)";

    /// <summary>
    /// Array of connections published in the payload.
    /// </summary>
    public List<Connection> Connections { get; set; } = [];

    /// <summary>
    /// HashVersion. e.g. 37638866
    /// </summary>
    public long HashVersion { get; set; }

    /// <summary>
    /// Unique sequence number of the payload. e.g. 1
    /// </summary>
    public long Seq { get; set; }

    /// <summary>
    /// e.g. "ie/s/j/simatic/v1/mbtcp1/status"
    /// </summary>
    public string StatusTopic { get; set; } = null!;
}
namespace Faoem.OpcUaConnector.Definitions;

public class MetaData
{
    /// <summary>
    /// e.g. "OPC UA Connector"
    /// </summary>
    public string ApplicationName { get; set; } = "OPC UA Connector";

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
    /// e.g. "ie/s/j/simatic/v1/opcuac1/status"
    /// </summary>
    public string StatusTopic { get; set; } = null!;
}
namespace Faoem.S7Connector.Definitions;

public class MetaData
{
    /// <summary>
    /// e.g. "SIMATIC S7 Connector"
    /// </summary>
    public string ApplicationName { get; set; } = "SIMATIC S7 Connector";

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
    /// e.g. "ie/s/j/simatic/v1/s7c1/status"
    /// </summary>
    public string StatusTopic { get; set; } = null!;
}
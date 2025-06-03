namespace Faoem.S7Connector.Definitions;

public class Connection
{
    /// <summary>
    /// Connection name. e.g. "1507d"
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Connection type. e.g. "S7Plus"
    /// </summary>
    public string Type { get; set; } = null!;

    /// <summary>
    /// Array of the connection data points.
    /// </summary>
    public List<DataPoint> DataPoints { get; set; } = null!;
}
namespace Faoem.ModbusTcpConnector.Definitions;

public class DataPoint
{
    /// <summary>
    /// Data point name. e.g. "1000ms"
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// e.g. "ie/d/j/simatic/v1/mbtcp1/dp/w/1507d"
    /// </summary>
    public string PubTopic { get; set; } = null!;

    /// <summary>
    /// Publish type of the data point. e.g. "timeseries"
    /// </summary>
    public string PublishType { get; set; } = null!;

    /// <summary>
    /// Topic on which the data point data is published. e.g. "ie/d/j/simatic/v1/mbtcp1/dp/r/1507d/1000ms"
    /// </summary>
    public string Topic { get; set; } = null!;

    /// <summary>
    /// Array of data point definitions.
    /// </summary>
    public List<DataPointDefinition> DataPointDefinitions { get; set; } = [];
}
namespace Faoem.OpcUaConnector.Definitions;

public class DataPoint
{
    /// <summary>
    /// Data point name. e.g. "default"
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// e.g. "ie/d/j/simatic/v1/opcuac1/dp/w/1507d"
    /// </summary>
    public string PubTopic { get; set; } = null!;

    /// <summary>
    /// Publish type of the data point. e.g. "bulk"
    /// </summary>
    public string PublishType { get; set; } = null!;

    /// <summary>
    /// Topic on which the data point data is published. e.g. "ie/d/j/simatic/v1/opcuac1/dp/r/1507d/default"
    /// </summary>
    public string Topic { get; set; } = null!;

    /// <summary>
    /// Array of data point definitions.
    /// </summary>
    public List<DataPointDefinition> DataPointDefinitions { get; set; } = [];
}
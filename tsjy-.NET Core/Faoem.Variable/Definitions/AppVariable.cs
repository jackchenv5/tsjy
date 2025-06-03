namespace Faoem.Variable.Definitions;

public class AppVariable(
    string accessMode,
    int acquisitionCycleInMs,
    string acquisitionMode,
    string dataType,
    string id,
    string name,
    bool isArray,
    string connectorInstance,
    string connectionName,
    string dataPointName
)
{
    public Guid Guid { get; set; } = Guid.NewGuid();
    public string AccessMode { get; set; } = accessMode;
    public int AcquisitionCycleInMs { get; set; } = acquisitionCycleInMs;
    public string AcquisitionMode { get; set; } = acquisitionMode;
    public string DataType { get; set; } = dataType;
    public string Id { get; set; } = id;
    public string Name { get; set; } = name;
    public bool IsArray { get; set; } = isArray;
    public string ConnectorInstance { get; set; } = connectorInstance;
    public string ConnectionName { get; set; } = connectionName;
    public string DataPointName { get; set; } = dataPointName;

    public AppQualityCode? Qc { get; set; }
    public DateTimeOffset? Ts { get; set; }
    public dynamic? Val { get; set; }
}
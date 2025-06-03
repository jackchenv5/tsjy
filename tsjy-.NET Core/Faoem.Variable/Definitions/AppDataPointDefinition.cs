namespace Faoem.Variable.Definitions;

public class AppDataPointDefinition(
    string accessMode,
    int acquisitionCycleInMs,
    string acquisitionMode,
    string dataType,
    string id,
    string name,
    bool isArray,
    string connectorInstance,
    string connectionName,
    string dataPointName)
{
    /// <summary>
    /// e.g. "rw"
    /// </summary>
    public string AccessMode { get; set; } = accessMode;

    /// <summary>
    /// e.g. 1000
    /// </summary>
    public int AcquisitionCycleInMs { get; set; } = acquisitionCycleInMs;

    /// <summary>
    /// e.g. "CyclicOnChange"
    /// * Modbus TCP Connector 没有这个属性
    /// </summary>
    public string AcquisitionMode { get; set; } = acquisitionMode;

    /// <summary>
    /// Data point type. e.g. "Int", "Bool", "Real"
    /// </summary>
    public string DataType { get; set; } = dataType;

    /// <summary>
    /// Data point id. e.g. "101"
    /// </summary>
    public string Id { get; set; } = id;

    /// <summary>
    /// Data point name. e.g. "Db_4.Int_Count", "Db_4.Real_Count", "测试.一."结构体.1"."结构体.变量.1"", "测试.一.Static_1", "测试.一."结构体.1".Static_1"
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// 是否为数组。目前（2024-7-12） OPC UA Connector 支持数组。仅支持一维数组
    /// </summary>
    public bool IsArray { get; set; } = isArray;

    public string ConnectorInstance { get; set; } = connectorInstance;
    public string ConnectionName { get; set; } = connectionName;
    public string DataPointName { get; set; } = dataPointName;
}
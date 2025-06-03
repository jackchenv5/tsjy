namespace Faoem.S7Connector.Definitions;

public class DataPointDefinition
{
    /// <summary>
    /// e.g. "rw"
    /// </summary>
    public string AccessMode { get; set; } = null!;

    /// <summary>
    /// e.g. 1000
    /// </summary>
    public int AcquisitionCycleInMs { get; set; }

    /// <summary>
    /// e.g. "CyclicOnChange"
    /// </summary>
    public string AcquisitionMode { get; set; } = null!;

    /// <summary>
    /// Data point type. e.g. "Int", "Bool", "Real"
    /// </summary>
    public string DataType { get; set; } = null!;

    /// <summary>
    /// Data point id. e.g. "101"
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Data point name. e.g. "Db_4.Int_Count", "Db_4.Real_Count", "测试.一."结构体.1"."结构体.变量.1"", "测试.一.Static_1", "测试.一."结构体.1".Static_1"
    /// </summary>
    public string Name { get; set; } = null!;
}
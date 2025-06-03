namespace Faoem.ModbusTcpConnector.Definitions;

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
    /// Data point type. e.g. "Int", "Bool", "Real"
    /// </summary>
    public string DataType { get; set; } = null!;

    /// <summary>
    /// Data point id. e.g. "101"
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Data point name. e.g. "Hd_切割总时间Hour"
    /// </summary>
    public string Name { get; set; } = null!;
}
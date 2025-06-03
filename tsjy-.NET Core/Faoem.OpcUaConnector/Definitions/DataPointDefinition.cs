namespace Faoem.OpcUaConnector.Definitions;

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
    /// Data point name. e.g. "Root.Objects.Controller.DataBlocksGlobal.HMI_Global.HMI_Dsp_Current_LineVelocity"
    /// </summary>
    public string Name { get; set; } = null!;
    
    public List<dynamic>? ArrayDimensions { get; set; }
    public int? ValueRank { get; set; }
}
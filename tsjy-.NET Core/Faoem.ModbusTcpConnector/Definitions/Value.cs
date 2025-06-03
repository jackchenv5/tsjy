namespace Faoem.ModbusTcpConnector.Definitions;

public class Value
{
    public string Id { get; set; } = null!;
    public QualityCode Qc { get; set; }
    public dynamic Val { get; set; } = null!;
}
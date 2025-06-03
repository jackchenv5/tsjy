namespace Faoem.ModbusTcpConnector.Definitions;

public class Record
{
    public long Rseq { get; set; }
    public DateTimeOffset Ts { get; set; }
    public List<Value> Vals { get; set; } = null!;
}
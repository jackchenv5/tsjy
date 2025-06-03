namespace Faoem.S7Connector.Definitions;

public class Data
{
    public long Seq { get; set; }
    public List<Value> Vals { get; set; } = null!;
}
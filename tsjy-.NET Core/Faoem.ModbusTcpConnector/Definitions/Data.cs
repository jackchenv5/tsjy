namespace Faoem.ModbusTcpConnector.Definitions;

public class Data
{
    public long Seq { get; set; }
    public long MdHashVer { get; set; }
    public List<Record> Records { get; set; } = null!;
}
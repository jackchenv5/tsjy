namespace Tsjy.Definitions;

public class ProductionData
{
    public string CustomerCode { get; set; } = string.Empty;
    public string MaterialSpecification { get; set; } = string.Empty;
    public bool StartSignal { get; set; }
    public bool CompleteSignal { get; set; }
}
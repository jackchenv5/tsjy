namespace Faoem.Variable.Inputs;

public class VariableFilterInput
{
    public string? ConnectorInstance { get; set; }
    public string? ConnectionName { get; set; }
    public string? DataPointName { get; set; }
    public string? Name { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
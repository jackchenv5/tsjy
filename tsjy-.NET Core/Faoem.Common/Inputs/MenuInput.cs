namespace Faoem.Common.Inputs;

public class MenuInput
{
    public string Label { get; set; } = null!;
    public bool IsSubMenu { get; set; }
    public int Order { get; set; }
    public long? ParentId { get; set; }
    public string? Route { get; set; }
}
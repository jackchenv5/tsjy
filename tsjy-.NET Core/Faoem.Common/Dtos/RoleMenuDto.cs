namespace Faoem.Common.Dtos;

public class RoleMenuDto
{
    public long Id { get; set; }
    public string Label { get; set; } = null!;
    public bool IsSubMenu { get; set; }
    public int Order { get; set; }
    public long? ParentId { get; set; }
    public string? Route { get; set; }
    public List<RoleMenuDto>? Children { get; set; }
    public bool HasMenu { get; set; }
}
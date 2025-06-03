namespace Faoem.Common.Dtos;

public class RolePermissionDto
{
    public Guid PermissionId { get; set; }
    public string Route { get; set; } = null!;
    public string HttpMethod { get; set; } = null!;
    public string ControllerName { get; set; } = null!;
    public string? ActionDescription { get; set; }
    public bool HasPermission { get; set; }
}
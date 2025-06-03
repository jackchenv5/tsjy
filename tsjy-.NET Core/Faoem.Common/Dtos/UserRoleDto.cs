namespace Faoem.Common.Dtos;

public class UserRoleDto
{
    public long RoleId { get; set; }
    public string RoleName { get; set; } = null!;
    public bool HasRole { get; set; }
}
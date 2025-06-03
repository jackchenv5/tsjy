using Faoem.Common.Dtos;

namespace Faoem.Common.Services.RolePermission;

public interface IRolePermissionService
{
    public Task<List<RolePermissionDto>> GetRolePermissionAsync(long roleId);
    public Task UpdateRolePermissionAsync(long roleId, List<Guid> permissionIds);

}
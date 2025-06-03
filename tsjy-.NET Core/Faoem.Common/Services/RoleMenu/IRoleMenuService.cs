using Faoem.Common.Dtos;

namespace Faoem.Common.Services.RoleMenu;

public interface IRoleMenuService
{
    public Task<List<Models.Menu>> GetUserMenuAsync();

    public Task<List<RoleMenuDto>> GetRoleMenuAsync(long roleId);

    public Task UpdateRoleMenuAsync(long roleId, List<long> menuIds);
}
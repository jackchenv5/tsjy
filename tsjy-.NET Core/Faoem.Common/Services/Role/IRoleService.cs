using Faoem.Common.Dtos;
using Faoem.Common.Inputs;

namespace Faoem.Common.Services.Role;

public interface IRoleService
{
    public Task<PagedDto<Models.Role>> GetRoleAsync(int pageIndex = 1, int pageSize = 20);

    public Task<Models.Role?> GetRoleAsync(long roleId);

    public Task<Models.Role> AddRoleAsync(RoleInput roleInput);

    public Task UpdateRoleAsync(long roleId, RoleInput roleInput);

    public Task DeleteRoleAsync(long roleId);

    public Task<Models.Role> GetSysAdminRole();
}
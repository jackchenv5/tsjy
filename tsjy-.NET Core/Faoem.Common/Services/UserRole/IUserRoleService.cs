using Faoem.Common.Dtos;

namespace Faoem.Common.Services.UserRole;

public interface IUserRoleService
{
    public Task<List<UserRoleDto>> GetUserRoleDtoAsync(long userId);
    public Task UpdateUserRolesAsync(long userId, List<long> roleIds);
    public Task<List<Models.Role>> GetUserRolesAsync(long userId);
    public Task<List<UserDto>> GetUsersAsync(long roleId);
}
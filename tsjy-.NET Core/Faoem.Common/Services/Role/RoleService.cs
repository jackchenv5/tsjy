using Faoem.Common.DbContexts;
using Faoem.Common.Dtos;
using Faoem.Common.Exceptions;
using Faoem.Common.Inputs;
using Microsoft.EntityFrameworkCore;

namespace Faoem.Common.Services.Role;

internal class RoleService(CommonDbContext commonDbContext) : IRoleService
{
    public async Task<PagedDto<Models.Role>> GetRoleAsync(int pageIndex = 1, int pageSize = 20)
    {
        var total = await commonDbContext.Roles.CountAsync();
        var items = await commonDbContext.Roles
            .OrderBy(r => r.Id)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedDto<Models.Role>(total, items);
    }

    public async Task<Models.Role?> GetRoleAsync(long roleId)
    {
        var role = await commonDbContext.Roles.FindAsync(roleId);

        return role ?? default;
    }

    public async Task<Models.Role> AddRoleAsync(RoleInput roleInput)
    {
        var lowerRoleName = roleInput.RoleName.ToLower();
        if (await commonDbContext.Roles.AnyAsync(r => r.LowerRoleName == lowerRoleName))
        {
            throw new AppException("The role name is in used.", 400);
        }

        var role = new Models.Role
        {
            RoleName = roleInput.RoleName
        };

        await commonDbContext.Roles.AddAsync(role);
        await commonDbContext.SaveChangesAsync();

        return role;
    }

    public async Task UpdateRoleAsync(long roleId, RoleInput roleInput)
    {
        var role = await commonDbContext.Roles.FindAsync(roleId);

        if (role is null)
        {
            throw new AppException("The role is not found.", 404);
        }

        if (role.LowerRoleName == "sysadmin")
        {
            throw new AppException("You are not allowed to modify the sysadmin role.", 403);
        }

        role.RoleName = roleInput.RoleName;
        await commonDbContext.SaveChangesAsync();
    }

    public async Task DeleteRoleAsync(long roleId)
    {
        var role = await commonDbContext.Roles.FindAsync(roleId);

        if (role is null)
        {
            throw new AppException("The role is not found.", 404);
        }

        if (role.LowerRoleName == "sysadmin")
        {
            throw new AppException("You are not allowed to delete the sysadmin role.", 403);
        }

        commonDbContext.Roles.Remove(role);
        await commonDbContext.SaveChangesAsync();
    }

    public async Task<Models.Role> GetSysAdminRole()
    {
        var role = await commonDbContext.Roles.FirstOrDefaultAsync(r => r.LowerRoleName == "sysadmin");

        if (role is null)
        {
            throw new Exception("SysAdmin role is required.");
        }

        return role;
    }
}
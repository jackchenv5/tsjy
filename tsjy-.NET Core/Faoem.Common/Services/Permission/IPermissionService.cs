namespace Faoem.Common.Services.Permission;

public interface IPermissionService
{
    public Task<bool> CheckPermissionAsync();
    public Task RefreshPermissionAsync();
}
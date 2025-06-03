using Faoem.Common.Inputs;

namespace Faoem.Common.Services.Menu;

public interface IMenuService
{
    /// <summary>
    /// 获取结构化的菜单
    /// </summary>
    /// <returns></returns>
    public Task<List<Models.Menu>> GetMenuAsync();

    /// <summary>
    /// 获取菜单列表
    /// </summary>
    /// <returns></returns>
    public Task<List<Models.Menu>> GetMenusAsync();

    public Task<Models.Menu> GetMenuAsync(long menuId);

    public Task<Models.Menu> AddMenuAsync(MenuInput menuInput);

    public Task UpdateMenuAsync(long menuId, MenuInput menuInput);

    public Task UpdateMenuAsync(List<Models.Menu> menus);

    public Task DeleteMenuAsync(long menuId);
}
using Faoem.Common.Inputs;
using Faoem.Common.Services.Menu;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Faoem.FacilityStatus.Extensions;

internal static class SeedDataExtension
{
    internal static WebApplication AddDefaultMenu(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var sp = scope.ServiceProvider;
        var menuService = sp.GetRequiredService<IMenuService>();
        var menus = menuService.GetMenusAsync().Result;
        const string facilityStatusRoute = "/facility-status";
        var facilityStatusMenu = menus.FirstOrDefault(menu => menu.LowerRoute == facilityStatusRoute);
        if (facilityStatusMenu is not null)
        {
            return app;
        }

        var order = menus.Count != 0 ? menus.Max(menu => menu.Order) : 0;
        facilityStatusMenu = menuService.AddMenuAsync(new MenuInput
        {
            IsSubMenu = true,
            Label = "Facility Status",
            Order = ++order,
            Route = facilityStatusRoute,
        }).Result;
        menuService.AddMenuAsync(new MenuInput
        {
            IsSubMenu = false,
            Label = "Status Summary",
            Order = ++order,
            ParentId = facilityStatusMenu.Id,
            Route = $"{facilityStatusRoute}/status-summary",
        }).Wait();
        menuService.AddMenuAsync(new MenuInput
        {
            IsSubMenu = false,
            Label = "Stopped Data",
            Order = ++order,
            ParentId = facilityStatusMenu.Id,
            Route = $"{facilityStatusRoute}/stopped-data",
        }).Wait();
        menuService.AddMenuAsync(new MenuInput
        {
            IsSubMenu = false,
            Label = "status-binding",
            Order = ++order,
            ParentId = facilityStatusMenu.Id,
            Route = $"{facilityStatusRoute}/status-binding",
        }).Wait();

        return app;
    }
}
using Faoem.Common.Inputs;
using Faoem.Common.Services.Menu;
using Faoem.Facility.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Faoem.Facility.Extensions;

internal static class SeedDataExtension
{
    internal static FacilityDbContext AddSeedData(this FacilityDbContext facilityDbContext)
    {
        facilityDbContext.AddFacilitySeedData();

        return facilityDbContext;
    }

    private static void AddFacilitySeedData(this FacilityDbContext facilityDbContext)
    {
        if (facilityDbContext.Facilities.Any())
        {
            return;
        }

        for (var i = 1; i < 9; i++)
        {
            var facility = new Models.Facility
            {
                Name = $"Facility {i}",
                IsEnabled = true
            };
            facilityDbContext.Facilities.Add(facility);
        }

        facilityDbContext.SaveChanges();
    }

    internal static WebApplication AddDefaultMenu(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var sp = scope.ServiceProvider;
        var menuService = sp.GetRequiredService<IMenuService>();
        var menus = menuService.GetMenusAsync().Result;
        var settingsMenu = menus.FirstOrDefault(menu => menu.LowerRoute == "/settings");
        if (settingsMenu is null)
        {
            return app;
        }

        const string facilitySettingRoute = "/settings/facility";
        if (menus.Any(menu => menu.LowerRoute == facilitySettingRoute))
        {
            return app;
        }

        var order = menus.Count != 0 ? menus.Max(menu => menu.Order) : 0;
        var facilitySettingMenu = new MenuInput
        {
            IsSubMenu = false,
            Label = "Facility Setting",
            Order = ++order,
            ParentId = settingsMenu.Id,
            Route = facilitySettingRoute
        };

        menuService.AddMenuAsync(facilitySettingMenu).Wait();

        return app;
    }
}
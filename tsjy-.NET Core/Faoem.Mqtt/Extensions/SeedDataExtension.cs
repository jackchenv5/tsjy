using Faoem.Common.Inputs;
using Faoem.Common.Services.Menu;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Faoem.Mqtt.Extensions;

internal static class SeedDataExtension
{
    internal static WebApplication AddDefaultMenu(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var sp = scope.ServiceProvider;
        var menuService = sp.GetRequiredService<IMenuService>();

        var menus = menuService.GetMenuAsync().Result;

        if (menus.Any(m => m.Route == "/mqtt/test"))
        {
            return app;
        }

        var order = menus.Count != 0 ? menus.OrderBy(m => m.Order).Last().Order : 0;

        menuService.AddMenuAsync(new MenuInput
        {
            IsSubMenu = false,
            Label = "Mqtt Test",
            Order = ++order,
            Route = "/mqtt/test"
        }).Wait();

        return app;
    }
}
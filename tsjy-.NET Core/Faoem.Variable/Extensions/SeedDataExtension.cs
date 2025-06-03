using Faoem.Common.Inputs;
using Faoem.Common.Services.Menu;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Faoem.Variable.Extensions;

internal static class SeedDataExtension
{
    internal static WebApplication AddDefaultMenu(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var sp = scope.ServiceProvider;
        var menuService = sp.GetRequiredService<IMenuService>();

        var menus = menuService.GetMenuAsync().Result;

        if (menus.Any(m => m.Route == "/variable"))
        {
            return app;
        }

        var order = menus.Count != 0 ? menus.OrderBy(m => m.Order).Last().Order : 0;

        var variableMenu = menuService.AddMenuAsync(new MenuInput
        {
            IsSubMenu = true,
            Label = "Variable",
            Order = ++order,
            Route = "/variable"
        }).Result;

        menuService.AddMenuAsync(new MenuInput
        {
            Label = "Connector Status",
            Order = ++order,
            ParentId = variableMenu.Id,
            Route = "/variable/connector-status"
        }).Wait();

        menuService.AddMenuAsync(new MenuInput
        {
            Label = "Monitor",
            Order = ++order,
            ParentId = variableMenu.Id,
            Route = "/variable/monitor"
        }).Wait();

        menuService.AddMenuAsync(new MenuInput
        {
            Label = "Archive",
            Order = ++order,
            ParentId = variableMenu.Id,
            Route = "/variable/archive"
        }).Wait();

        return app;
    }
}
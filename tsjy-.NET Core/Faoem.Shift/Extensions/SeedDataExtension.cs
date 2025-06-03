using Faoem.Common.Inputs;
using Faoem.Common.Services.Menu;
using Faoem.Shift.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Faoem.Shift.Extensions;

internal static class SeedDataExtension
{
    internal static ShiftDbContext AddSeedData(this ShiftDbContext shiftDbContext)
    {
        shiftDbContext.AddShiftSeedData();

        return shiftDbContext;
    }

    private static void AddShiftSeedData(this ShiftDbContext shiftDbContext)
    {
        if (shiftDbContext.Shifts.Any())
        {
            return;
        }

        var now = DateTimeOffset.Now;
        for (byte i = 1; i < 4; i++)
        {
            var shift = new Models.Shift
            {
                Number = i,
                IsEnabled = true,
                Name = $"Shift {i}",
                StartTime = new DateTimeOffset(
                        now.Year,
                        now.Month,
                        now.Day + ((i - 1) * 8 / 24),
                        (i - 1) * 8 % 24,
                        0,
                        0,
                        now.Offset
                    )
                    .ToUnixTimeSeconds(),
                EndTime = new DateTimeOffset(
                        now.Year,
                        now.Month,
                        now.Day + i * 8 / 24,
                        i * 8 % 24,
                        0,
                        0,
                        now.Offset
                    )
                    .ToUnixTimeSeconds(),
                SpanTheDay = false
            };
            shiftDbContext.Shifts.Add(shift);
        }

        shiftDbContext.SaveChanges();
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

        if (menus.Any(menu => menu.Route == "/settings/shift"))
        {
            return app;
        }

        var order = menus.Count != 0 ? menus.OrderBy(menu => menu.Order).Last().Order : 0;

        var shiftMenu = new MenuInput
        {
            IsSubMenu = false,
            Label = "Shift Setting",
            Order = ++order,
            ParentId = settingsMenu.Id,
            Route = "/settings/shift"
        };

        menuService.AddMenuAsync(shiftMenu).Wait();

        return app;
    }
}
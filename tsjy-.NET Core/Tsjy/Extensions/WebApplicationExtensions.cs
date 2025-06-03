using Faoem.Common.Inputs;
using Faoem.Common.Services.Menu;

namespace Tsjy.Extensions;

internal static class WebApplicationExtensions
{
    internal static WebApplication ConfigureDefaultMenus(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var menuService = serviceProvider.GetRequiredService<IMenuService>();
        var menus = menuService.GetMenusAsync().Result;
        var order = menus.Count != 0 ? menus.Max(menu => menu.Order) : 0;

        var needInitMenus = false;

        #region 设备数据

        // 设备状态插件会自动创建
        var facilityDataMenu = menus.First(menu => menu.Route == "/facility-status");
        menuService.UpdateMenuAsync(facilityDataMenu.Id, new MenuInput()
        {
            IsSubMenu = true,
            Label = "设备数据",
            Order = ++order,
            Route = "/facility-status"
        });
        if (menus.FirstOrDefault(menu => menu.Route == "/tsjy-facility-status/status") is null)
        {
            // 如果没有切割机监控菜单，需要重新初始化菜单
            needInitMenus = true;

            menuService.AddMenuAsync(new MenuInput
            {
                IsSubMenu = false,
                Label = "切割机监控",
                Order = ++order,
                Route = "/tsjy-facility-status/status",
                ParentId = facilityDataMenu.Id
            });
        }

        if (menus.FirstOrDefault(menu => menu.Route == "/tsjy-motor/data") is null)
        {
            menuService.AddMenuAsync(new MenuInput
            {
                IsSubMenu = false,
                Label = "电机数据",
                Order = ++order,
                Route = "/tsjy-motor/data",
                ParentId = facilityDataMenu.Id
            });
        }

        if (menus.FirstOrDefault(menu => menu.Route == "/tsjy-alarm/current-alarms") is null)
        {
            menuService.AddMenuAsync(new MenuInput
            {
                IsSubMenu = false,
                Label = "当前报警",
                Order = ++order,
                Route = "/tsjy-alarm/current-alarms",
                ParentId = facilityDataMenu.Id
            });
        }

        if (menus.FirstOrDefault(menu => menu.Route == "/tsjy-alarm/alarms-history") is null)
        {
            menuService.AddMenuAsync(new MenuInput
            {
                IsSubMenu = false,
                Label = "报警历史",
                Order = ++order,
                Route = "/tsjy-alarm/alarms-history",
                ParentId = facilityDataMenu.Id
            });

            // 由模块创建
            var statusSummaryMenu = menus.First(menu => menu.Route == "/facility-status/status-summary");
            menuService.UpdateMenuAsync(statusSummaryMenu.Id, new MenuInput
            {
                IsSubMenu = false,
                Label = "状态统计",
                Order = ++order,
                Route = "/facility-status/status-summary",
                ParentId = facilityDataMenu.Id
            });
            // 由模块创建
            var stoppedDataMenu = menus.First(menu => menu.Route == "/facility-status/stopped-data");
            menuService.UpdateMenuAsync(stoppedDataMenu.Id, new MenuInput
            {
                IsSubMenu = false,
                Label = "停机数据统计",
                Order = ++order,
                Route = "/facility-status/stopped-data",
                ParentId = facilityDataMenu.Id
            });
        }

        #endregion

        #region 生产数据

        var productionDataMenu = menus.FirstOrDefault(menu => menu.Route == "/tsjy-production");
        if (productionDataMenu is null)
        {
            productionDataMenu = menuService.AddMenuAsync(new MenuInput
            {
                IsSubMenu = true,
                Label = "生产数据",
                Order = ++order,
                Route = "/tsjy-production",
            }).Result;
        }

        if (menus.FirstOrDefault(menu => menu.Route == "/tsjy-production/history") is null)
        {
            menuService.AddMenuAsync(new MenuInput
            {
                IsSubMenu = false,
                Label = "生产历史",
                Order = ++order,
                Route = "/tsjy-production/history",
                ParentId = productionDataMenu.Id
            });
        }

        if (menus.FirstOrDefault(menu => menu.Route == "/tsjy-craft/history") is null)
        {
            menuService.AddMenuAsync(new MenuInput
            {
                IsSubMenu = false,
                Label = "工艺历史",
                Order = ++order,
                Route = "/tsjy-craft/history",
                ParentId = productionDataMenu.Id
            });
        }

        #endregion

        #region 维护列表

        var partMenu = menus.FirstOrDefault(menu => menu.Route == "/tsjy-part");
        if (partMenu is null)
        {
            partMenu = menuService.AddMenuAsync(new MenuInput
            {
                IsSubMenu = true,
                Label = "维护列表",
                Order = ++order,
                Route = "/tsjy-part",
            }).Result;
        }

        if (menus.FirstOrDefault(menu => menu.Route == "/tsjy-part/list") is null)
        {
            menuService.AddMenuAsync(new MenuInput
            {
                IsSubMenu = false,
                Label = "零件列表",
                Order = ++order,
                Route = "/tsjy-part/list",
                ParentId = partMenu.Id
            });
        }

        if (menus.FirstOrDefault(menu => menu.Route == "/tsjy-part/history") is null)
        {
            menuService.AddMenuAsync(new MenuInput
            {
                IsSubMenu = false,
                Label = "维护历史",
                Order = ++order,
                Route = "/tsjy-part/history",
                ParentId = partMenu.Id
            });
        }

        #endregion

        #region 应用配置

        var configureMenu = menus.FirstOrDefault(menu => menu.Route == "/tsjy-configure");
        if (configureMenu is null)
        {
            configureMenu = menuService.AddMenuAsync(new MenuInput
            {
                IsSubMenu = true,
                Label = "应用配置",
                Order = ++order,
                Route = "/tsjy-configure",
            }).Result;
            // 由模块创建
            var statusBindingMenu = menus.First(menu => menu.Route == "/facility-status/status-binding");
            menuService.UpdateMenuAsync(statusBindingMenu.Id, new MenuInput
            {
                IsSubMenu = false,
                Label = "状态变量配置",
                Order = ++order,
                Route = "/facility-status/status-binding",
                ParentId = configureMenu.Id
            });
        }

        if (menus.FirstOrDefault(menu => menu.Route == "/tsjy-facility-status/status-binding") is null)
        {
            menuService.AddMenuAsync(new MenuInput
            {
                IsSubMenu = false,
                Label = "监控变量配置",
                Order = ++order,
                Route = "/tsjy-facility-status/status-binding",
                ParentId = configureMenu.Id
            });
        }

        if (menus.FirstOrDefault(menu => menu.Route == "/tsjy-production/setting") is null)
        {
            menuService.AddMenuAsync(new MenuInput
            {
                IsSubMenu = false,
                Label = "生产变量配置",
                Order = ++order,
                Route = "/tsjy-production/setting",
                ParentId = configureMenu.Id
            });
        }

        if (menus.FirstOrDefault(menu => menu.Route == "/tsjy-craft/binding") is null)
        {
            menuService.AddMenuAsync(new MenuInput
            {
                IsSubMenu = false,
                Label = "工艺变量配置",
                Order = ++order,
                Route = "/tsjy-craft/binding",
                ParentId = configureMenu.Id
            });
        }

        if (menus.FirstOrDefault(menu => menu.Route == "/tsjy-alarm/definition") is null)
        {
            menuService.AddMenuAsync(new MenuInput
            {
                IsSubMenu = false,
                Label = "报警定义",
                Order = ++order,
                Route = "/tsjy-alarm/definition",
                ParentId = configureMenu.Id
            });
        }

        if (menus.FirstOrDefault(menu => menu.Route == "/tsjy-motor/setting") is null)
        {
            menuService.AddMenuAsync(new MenuInput
            {
                IsSubMenu = false,
                Label = "电机状态配置",
                Order = ++order,
                Route = "/tsjy-motor/setting",
                ParentId = configureMenu.Id
            });
        }

        if (menus.FirstOrDefault(menu => menu.Route == "/tsjy-part/binding") is null)
        {
            menuService.AddMenuAsync(new MenuInput
            {
                IsSubMenu = false,
                Label = "易耗品配置",
                Order = ++order,
                Route = "/tsjy-part/binding",
                ParentId = configureMenu.Id
            });

            // 由模块创建
            var facilityMenu = menus.First(menu => menu.Route == "/settings/facility");
            menuService.UpdateMenuAsync(facilityMenu.Id, new MenuInput
            {
                IsSubMenu = false,
                Label = "设备配置",
                Order = ++order,
                Route = "/settings/facility",
                ParentId = configureMenu.Id
            });
        }

        #endregion

        #region 系统设置

        if (needInitMenus)
        {
            var settingsMenu = menus.First(menu => menu.Route == "/settings");
            menuService.UpdateMenuAsync(settingsMenu.Id, new MenuInput()
            {
                IsSubMenu = true,
                Label = "系统设置",
                Order = ++order,
                Route = "/settings"
            });
            var appSettingMenu = menus.First(menu => menu.Route == "/settings/app");
            menuService.UpdateMenuAsync(appSettingMenu.Id, new MenuInput
            {
                IsSubMenu = false,
                Label = "应用设置",
                Order = ++order,
                Route = "/settings/app",
                ParentId = settingsMenu.Id
            });
            var menuSettingMenu = menus.First(menu => menu.Route == "/settings/menu");
            menuService.UpdateMenuAsync(menuSettingMenu.Id, new MenuInput
            {
                IsSubMenu = false,
                Label = "菜单设置",
                Order = ++order,
                Route = "/settings/menu",
                ParentId = settingsMenu.Id
            });
            var roleSettingMenu = menus.First(menu => menu.Route == "/settings/role");
            menuService.UpdateMenuAsync(roleSettingMenu.Id, new MenuInput
            {
                IsSubMenu = false,
                Label = "角色设置",
                Order = ++order,
                Route = "/settings/role",
                ParentId = settingsMenu.Id
            });
            var userSettingMenu = menus.First(menu => menu.Route == "/settings/user");
            menuService.UpdateMenuAsync(userSettingMenu.Id, new MenuInput
            {
                IsSubMenu = false,
                Label = "用户设置",
                Order = ++order,
                Route = "/settings/user",
                ParentId = settingsMenu.Id
            });
            var userInfoSettingMenu = menus.First(menu => menu.Route == "/settings/user-profile");
            menuService.UpdateMenuAsync(userInfoSettingMenu.Id, new MenuInput
            {
                IsSubMenu = false,
                Label = "用户信息",
                Order = ++order,
                Route = "/settings/user-profile",
                ParentId = settingsMenu.Id
            });
            var shiftSettingMenu = menus.First(menu => menu.Route == "/settings/shift");
            menuService.UpdateMenuAsync(shiftSettingMenu.Id, new MenuInput
            {
                IsSubMenu = false,
                Label = "班次设置",
                Order = ++order,
                Route = "/settings/shift",
                ParentId = settingsMenu.Id
            });
        }

        #endregion

        #region 变量管理

        if (needInitMenus)
        {
            var variableMenu = menus.First(menu => menu.Route == "/variable");
            menuService.UpdateMenuAsync(variableMenu.Id, new MenuInput()
            {
                IsSubMenu = true,
                Label = "变量管理",
                Order = ++order,
                Route = "/variable"
            });
            var connectorStatusMenu = menus.First(menu => menu.Route == "/variable/connector-status");
            menuService.UpdateMenuAsync(connectorStatusMenu.Id, new MenuInput
            {
                IsSubMenu = false,
                Label = "连机器状态",
                Order = ++order,
                Route = "/variable/connector-status",
                ParentId = variableMenu.Id
            });
            var monitorMenu = menus.First(menu => menu.Route == "/variable/monitor");
            menuService.UpdateMenuAsync(monitorMenu.Id, new MenuInput
            {
                IsSubMenu = false,
                Label = "变量监控",
                Order = ++order,
                Route = "/variable/monitor",
                ParentId = variableMenu.Id
            });
            var archiveMenu = menus.First(menu => menu.Route == "/variable/archive");
            menuService.UpdateMenuAsync(archiveMenu.Id, new MenuInput
            {
                IsSubMenu = false,
                Label = "变量归档",
                Order = ++order,
                Route = "/variable/archive",
                ParentId = variableMenu.Id
            });
        }

        #endregion

        #region mqtt 测试

        if (needInitMenus)
        {
            var mqttTestMenu = menus.First(menu => menu.Route == "/mqtt/test");
            menuService.UpdateMenuAsync(mqttTestMenu.Id, new MenuInput
            {
                IsSubMenu = false,
                Label = "Mqtt 测试",
                Order = ++order,
                Route = "/mqtt/test"
            });
        }

        #endregion

        return app;
    }
}
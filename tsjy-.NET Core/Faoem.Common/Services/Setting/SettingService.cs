using Faoem.Common.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Faoem.Common.Services.Setting;

internal class SettingService(CommonDbContext commonDbContext) : ISettingService
{
    public async Task<Models.Setting> GetSettingAsync(string key)
    {
        var setting = await commonDbContext.Settings.FirstOrDefaultAsync(s => s.Key == key);

        if (setting is not null)
        {
            return setting;
        }

        setting = new Models.Setting
        {
            Key = key
        };

        await commonDbContext.Settings.AddAsync(setting);
        await commonDbContext.SaveChangesAsync();

        return setting;
    }

    public async Task UpdateSettingAsync(string key, string? value = default)
    {
        var setting = await commonDbContext.Settings.FirstOrDefaultAsync(s => s.Key == key);
        if (setting is null)
        {
            setting = new Models.Setting
            {
                Key = key
            };
            await commonDbContext.Settings.AddAsync(setting);
        }

        setting.Value = value;
        await commonDbContext.SaveChangesAsync();
    }
}
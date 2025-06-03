namespace Faoem.Common.Services.Setting;

public interface ISettingService
{
    public Task<Models.Setting> GetSettingAsync(string key);
    public Task UpdateSettingAsync(string key, string? value = default);
}
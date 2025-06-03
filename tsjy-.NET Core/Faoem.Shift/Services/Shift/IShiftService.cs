namespace Faoem.Shift.Services.Shift;

public interface IShiftService
{
    /// <summary>
    /// 获取当前班次
    /// </summary>
    /// <returns></returns>
    public Task<Models.Shift?> GetCurrentShiftAsync();

    /// <summary>
    /// 获取班次列表
    /// </summary>
    /// <returns></returns>
    public Task<List<Models.Shift>> GetShiftsAsync();

    /// <summary>
    /// 更新班次列表
    /// </summary>
    /// <param name="shifts"></param>
    /// <returns></returns>
    public Task UpdateShiftsAsync(List<Models.Shift> shifts);
}
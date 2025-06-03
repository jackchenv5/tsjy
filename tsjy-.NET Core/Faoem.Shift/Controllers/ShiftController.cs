using System.ComponentModel;
using Faoem.Shift.Services.Shift;
using Microsoft.AspNetCore.Mvc;

namespace Faoem.Shift.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShiftController(IShiftService shiftService) : ControllerBase
{
    [HttpGet("Current")]
    [Description("获取当前班次")]
    public async Task<ActionResult<Models.Shift?>> GetCurrentShiftAsync()
    {
        var shift = await shiftService.GetCurrentShiftAsync();
        return shift;
    }

    [HttpGet]
    [Description("获取班次列表")]
    public async Task<ActionResult<List<Models.Shift>>> GetAsync()
    {
        var shifts = await shiftService.GetShiftsAsync();
        return shifts;
    }

    [HttpPut]
    [Description("更新班次数据")]
    public async Task<ActionResult> UpdateAsync(List<Models.Shift> shifts)
    {
        await shiftService.UpdateShiftsAsync(shifts);
        return NoContent();
    }
}
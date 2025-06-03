using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Tsjy.Dtos;
using Tsjy.Models;
using Tsjy.Services;

namespace Tsjy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SawingMachineStatusController(
    StatusBindingService statusBindingService,
    StatusRecordService statusRecordService
) : ControllerBase
{
    [HttpGet("Binding/{facilityId}")]
    [Description("获取指定设备的状态绑定数据")]
    public async Task<ActionResult<List<TsjyStatusBinding>>> GetStatusBindingsAsync(long facilityId)
    {
        return await statusBindingService.GetStatusBindingsAsync(facilityId);
    }

    [HttpPut("Binding")]
    [Description("更新指定设备的状态绑定数据")]
    public async Task<IActionResult> UpdateStatusBindingAsync(TsjyStatusBinding tsjyStatusBinding)
    {
        await statusBindingService.UpdateStatusBindingAsync(tsjyStatusBinding);
        return NoContent();
    }

    [HttpGet("{facilityId}")]
    public async Task<ActionResult<SawingMachineStatus>> GetSawingMachineStatusAsync(long facilityId)
    {
        return await statusRecordService.GetSawingMachineStatusAsync(facilityId);
    }
}
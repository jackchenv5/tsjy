using InfluxDB.Client.Api.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tsjy.Definitions;
using Tsjy.Dtos;
using Tsjy.Models;
using Tsjy.Services;

namespace Tsjy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SawingMotorController(
    MotorService motorService,
    MotorBindingService motorBindingService,
    MotorRecordService motorRecordService
    
) : ControllerBase
{

    [HttpPost]
    public async Task<ActionResult<TsjyMotor>> AddMotorAsync(TsjyMotor input)
    {
        return await motorService.AddMotorAsync(input);
    }

    [HttpGet]
    public async Task<ActionResult<List<TsjyMotor>>> GetMotorsAsync(long facilityId)
    {
        return await motorService.GetMotorsAsync(facilityId);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateMotorAsync(TsjyMotor updateMotorDto)
    {
        await motorService.UpdateMotorAsync(updateMotorDto);
        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteMotorAsync(long motorId)
    {
        await motorService.DeleteMotorAsync(motorId);
        return NoContent();
    }

    [HttpGet("Binding/{motorId}")]
    public async Task<ActionResult<List<TsjyMotorBinding>>> GetMotorBindingsAsync(long motorId)
    {
        return await motorBindingService.GetMotorBindingsAsync(motorId);
    }

    [HttpPut("Binding")]
    public async Task<IActionResult> UpdateMotorBindingAsync(TsjyMotorBinding updateMotorBinding)
    {
        await motorBindingService.UpdateMotorBindingAsync(updateMotorBinding);
        return NoContent();
    }

    [HttpGet("Data")]
    public async Task<ActionResult<MotorDataDto>> GetMotorDataAsync(long motorId)
    {
        return await motorRecordService.GetMotorDataAsync(motorId);
    }

    [HttpGet("Data/History")]
    public async Task<ActionResult<GetMotorHistoryDto>> GetMotorsDataAsync(long motorId, long startTime, long endTime)
    {
        return await motorRecordService.GetMotorDataAsync(motorId, startTime, endTime);
    }
    // 新增 Action：获取卷径数据

}
using Faoem.Common.Dtos;
using Microsoft.AspNetCore.Mvc;
using Tsjy.Dtos;
using Tsjy.Models;
using Tsjy.Services;

namespace Tsjy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SawingCraftController(
    CraftBindingService craftBindingService,
    CraftRecordService craftRecordService
) : ControllerBase
{
    [HttpGet("Binding")]
    public async Task<ActionResult<List<CraftBindingDto>>> GetCraftBindingsAsync(long facilityId)
    {
        return await craftBindingService.GetCraftBindingsAsync(facilityId);
    }

    [HttpGet("AddGroup")]
    public async Task<ActionResult> AddCraftGroupAsync(long facilityId)
    {
        await craftBindingService.AddCraftGroupAsync(facilityId);
        return Ok();
    }

    [HttpDelete("DeleteGroup")]
    public async Task<ActionResult> DeleteCraftGroupAsync(long facilityId)
    {
        await craftBindingService.DeleteCraftGroupAsync(facilityId);
        return Ok();
    }

    [HttpPut("Binding")]
    public async Task<ActionResult> UpdateCraftBindingsAsync(TsjyCraftBinding updateBinding)
    {
        await craftBindingService.UpdateCraftBindingAsync(updateBinding);
        return NoContent();
    }

    [HttpPost("HistoryData")]
    public async Task<ActionResult<PagedDto<CraftDataDto>>> GetCraftDataAsync(GetCraftDataDto dto)
    {
        return await craftRecordService.GetCraftDataAsync(dto);
    }
}
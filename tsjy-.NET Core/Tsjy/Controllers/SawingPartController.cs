using Faoem.Common.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tsjy.Dtos;
using Tsjy.Models;
using Tsjy.Services;

namespace Tsjy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SawingPartController(PartRecordService partRecordService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TsjyPart>>> GetAsync(long facilityId)
    {
        return await partRecordService.GetPartsAsync(facilityId);
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync(List<TsjyPart> parts)
    {
        await partRecordService.AddPartAsync(parts);
        return Ok();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateAsync(TsjyPart part)
    {
        await partRecordService.UpdatePartAsync(part);
        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteAsync(long id)
    {
        await partRecordService.DeletePartAsync(id);
        return NoContent();
    }

    [HttpPost("GetMaintainHistory")]
    public async Task<ActionResult<PagedDto<TsjyPartMaintainHistory>>> GetMaintainHistoryAsync(
        GetPartMaintainHistoryDto dto
    )
    {
        return await partRecordService.GetPartMaintainHistoriesAsync(dto);
    }

    [HttpPost("AddMaintainHistory")]
    public async Task<ActionResult> AddMaintainHistoryAsync(TsjyPartMaintainHistory dto)
    {
        await partRecordService.AddPartMaintainHistoryAsync(dto);
        return NoContent();
    }
}
using Faoem.Common.Dtos;
using Faoem.FacilityStatus.Dtos;
using Faoem.FacilityStatus.Inputs;
using Faoem.FacilityStatus.Services.FacilityStatus;
using Microsoft.AspNetCore.Mvc;

namespace Faoem.FacilityStatus.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FacilityStatusController(IFacilityStatusService facilityStatusService) : ControllerBase
{
    [HttpPost("GetStatus")]
    public async Task<ActionResult<PagedDto<StatusRecordDto>>> GetAsync(StatusQueryInput statusQuery)
    {
        return await facilityStatusService.GetStatusAsync(statusQuery);
    }

    [HttpPost("GetAllStatus")]
    public async Task<ActionResult<List<StatusRecordDto>>> GetAllAsync(StatusQueryInput statusQuery)
    {
        return await facilityStatusService.GetAllStatusAsync(statusQuery);
    }

    [HttpGet("GetCurrentShiftStatus")]
    public async Task<ActionResult<StatusDto>> GetCurrentShiftStatusAsync(long facilityId)
    {
        return await facilityStatusService.GetCurrentShiftStatusAsync(facilityId);
    }
}
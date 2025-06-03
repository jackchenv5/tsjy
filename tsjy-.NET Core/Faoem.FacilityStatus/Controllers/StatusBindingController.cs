using Faoem.FacilityStatus.Models;
using Faoem.FacilityStatus.Services.StatusBindingService;
using Microsoft.AspNetCore.Mvc;

namespace Faoem.FacilityStatus.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatusBindingController(
    IStatusBindingService statusBindingService
) : ControllerBase
{
    [HttpGet("{facilityId}")]
    public async Task<ActionResult<List<StatusBinding>>> GetStatusBindingsAsync(long facilityId)
    {
        return await statusBindingService.GetStatusBindingsAsync(facilityId);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateStatusBindingAsync(StatusBinding statusBinding)
    {
        await statusBindingService.UpdateStatusBindingAsync(statusBinding);
        return NoContent();
    }
}
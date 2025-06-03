using System.ComponentModel;
using Faoem.Facility.Services.Facility;
using Microsoft.AspNetCore.Mvc;

namespace Faoem.Facility.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FacilityController(IFacilityService facilityService) : ControllerBase
{
    [HttpGet]
    [Description("获取设备列表")]
    public async Task<ActionResult<List<Models.Facility>>> GetAsync()
    {
        return await facilityService.GetFacilitiesAsync();
    }

    [HttpGet("{id}")]
    [Description("获取指定设备")]
    public async Task<ActionResult<Models.Facility>> GetAsync(long id)
    {
        return await facilityService.GetFacilityAsync(id);
    }

    [HttpPost]
    [Description("添加设备")]
    public async Task<ActionResult<Models.Facility>> AddAsync(Models.Facility facility)
    {
        return await facilityService.AddFacilityAsync(facility);
    }

    [HttpPut]
    [Description("更新指定设备")]
    public async Task<IActionResult> UpdateAsync(Models.Facility facility)
    {
        await facilityService.UpdateFacilityAsync(facility);
        return NoContent();
    }

    [HttpDelete]
    [Description("删除指定设备")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        await facilityService.DeleteFacilityAsync(id);
        return NoContent();
    }
}
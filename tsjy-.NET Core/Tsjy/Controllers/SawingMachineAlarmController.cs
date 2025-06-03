using Faoem.Common.Dtos;
using Microsoft.AspNetCore.Mvc;
using Tsjy.Dtos;
using Tsjy.Models;
using Tsjy.Services;

namespace Tsjy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SawingMachineAlarmController(
    AlarmDefinitionService alarmDefinitionService,
    AlarmService alarmService
) : ControllerBase
{
    [HttpGet("Definitions")]
    public async Task<ActionResult<PagedDto<TsjyAlarmDefinition>>> GetDefinitionsAsync(int pageIndex = 1,
        int pageSize = 20)
    {
        return await alarmDefinitionService.GetAlarmDefinitionsAsync(pageIndex, pageSize);
    }

    [HttpPost("Definition")]
    public async Task<ActionResult<TsjyAlarmDefinition>> AddDefinitionAsync(TsjyAlarmDefinition definition)
    {
        var result = await alarmDefinitionService.AddAlarmDefinitionAsync(definition);
        return result;
    }

    [HttpPost("Definitions")]
    public async Task<ActionResult> AddDefinitionAsync(List<TsjyAlarmDefinition> definitions)
    {
        await alarmDefinitionService.AddAlarmDefinitionsAsync(definitions);
        return Created();
    }

    [HttpPut("Definition")]
    public async Task<IActionResult> UpdateDefinitionAsync(TsjyAlarmDefinition definition)
    {
        await alarmDefinitionService.UpdateAlarmDefinitionAsync(definition);
        return NoContent();
    }

    [HttpDelete("Definition/{definitionId}")]
    public async Task<IActionResult> DeleteDefinitionAsync(long definitionId)
    {
        await alarmDefinitionService.DeleteAlarmDefinitionAsync(definitionId);
        return NoContent();
    }

    [HttpGet("CurrentAlarms")]
    public async Task<ActionResult<List<TsjyAlarmHistory>>> GetCurrentAlarmsAsync()
    {
        return await alarmService.GetCurrentAlarmsAsync();
    }

    [HttpPost("GetHistory")]
    public async Task<ActionResult<PagedDto<TsjyAlarmHistory>>> GetAlarmHistoryAsync(GetAlarmHistoryDto param)
    {
        return await alarmService.GetAlarmHistoryAsync(param);
    }

    [HttpPost("GetAlarmCount")]
    public async Task<ActionResult<List<AlarmCountDto>>> GetAlarmCountAsync(GetAlarmHistoryDto param)
    {
        return await alarmService.GetAlarmCountAsync(param);
    }
}
using System.ComponentModel;
using Faoem.Common.Dtos;
using Faoem.Variable.Inputs;
using Faoem.Variable.Models;
using Faoem.Variable.Services.VariableArchive;
using Microsoft.AspNetCore.Mvc;

namespace Faoem.Variable.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VariableArchiveController(IVariableArchiveService variableArchiveService) : ControllerBase
{
    [HttpPost("GetArchivedVariables")]
    [Description("获取归档变量列表")]
    public async Task<ActionResult<PagedDto<ArchivedVariable>>> GetArchivedVariablesAsync(
        [FromBody] VariableFilterInput filter
    )
    {
        return await variableArchiveService.GetArchivedVariablesAsync(filter);
    }

    /// <summary>
    /// 添加归档变量
    /// </summary>
    /// <param name="guid">要归档变量的 guid</param>
    /// <param name="archiveMode">archiveMode – 归档模式：0 - 变化时归档，1 - 周期归档</param>
    /// <returns></returns>
    [HttpPost("AddArchivedVariable")]
    [Description("添加归档变量")]
    public async Task<ActionResult<ArchivedVariable?>> AddArchivedVariableAsync(
        Guid guid,
        ArchiveMode archiveMode = ArchiveMode.Change
    )
    {
        var archivedVariable = await variableArchiveService.AddUserArchivedAsync(guid, Module.Name, archiveMode);
        if (archivedVariable is null)
        {
            return Ok();
        }

        return Created("", archivedVariable);
    }

    [HttpPut("UpdateArchivedVariable/{id}")]
    [Description("更新指定归档变量")]
    public async Task UpdateArchivedVariableAsync([FromRoute] long id, [FromBody] int interval)
    {
        await variableArchiveService.UpdateArchiveIntervalAsync(id, interval);
    }

    [HttpDelete("DeleteArchivedVariable/{id}")]
    [Description("删除指定归档变量")]
    public async Task<IActionResult> DeleteArchivedVariableAsync([FromRoute] long id)
    {
        await variableArchiveService.DeleteArchivedVariableAsync(id);

        return NoContent();
    }
}
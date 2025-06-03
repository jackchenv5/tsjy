using System.ComponentModel;
using Faoem.Common.Dtos;
using Faoem.Variable.Definitions;
using Faoem.Variable.Inputs;
using Faoem.Variable.Services.Variable;
using Microsoft.AspNetCore.Mvc;

namespace Faoem.Variable.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VariableController(IVariableService variableService) : ControllerBase
{
    [HttpGet("ConnectorStatus")]
    [Description("获取连接器状态")]
    public async Task<ActionResult<List<AppConnectorStatus>>> GetConnectorStatusAsync()
    {
        return await variableService.GetConnectorStatusAsync();
    }

    [HttpPost("GetVariables")]
    [Description("获取变量数据")]
    public async Task<ActionResult<PagedDto<AppVariable>>> GetDataPointDefinitionsAsync(
        [FromBody] VariableFilterInput filter
    )
    {
        return await variableService.GetVariablesAsync(filter);
    }
}
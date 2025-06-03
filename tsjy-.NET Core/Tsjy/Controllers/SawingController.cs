using Microsoft.AspNetCore.Mvc;
using Tsjy.Services;

namespace Tsjy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SawingController(TsjySingleService tsjySingleService) : ControllerBase
{
    [HttpGet("RequestAllVariable")]
    public async Task<IActionResult> RequestAllVariable()
    {
        await tsjySingleService.RequestAllVariableAsync();
        return Ok();
    }
}
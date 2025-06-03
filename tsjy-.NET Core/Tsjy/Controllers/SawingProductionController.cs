using Faoem.Common.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tsjy.Dtos;
using Tsjy.Models;
using Tsjy.Services;

namespace Tsjy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SawingProductionController : ControllerBase
{
    private readonly ProductionBindingService _productionBindingService;
    private readonly ProductionRecordService _productionRecordService;

    public SawingProductionController(
        ProductionBindingService productionBindingService,
        ProductionRecordService productionRecordService
    )
    {
        _productionBindingService = productionBindingService;
        _productionRecordService = productionRecordService;
    }

    [HttpGet("Bindings")]
    public async Task<ActionResult<List<TsjyProductionBinding>>> GetProductionBindingsAsync(long facilityId)
    {
        return await _productionBindingService.GetProductionBindingsAsync(facilityId);
    }

    [HttpPut("Bindings")]
    public async Task<ActionResult> UpdateProductionBindingsAsync(TsjyProductionBinding productionBinding)
    {
        await _productionBindingService.UpdateProductionBindingAsync(productionBinding);

        return NoContent();
    }

    [HttpPost("GetProductionHistory")]
    public async Task<ActionResult<PagedDto<TsjyProductionData>>> GetProductionHistoryAsync(GetProductionHistoryDto dto)
    {
        return await _productionRecordService.GetProductionHistoryAsync(dto);
    }

    [HttpPost("GetProductionStatistics")]
    public async Task<ActionResult<List<ProductionStatisticsDto>>> GetProductionStatisticsAsync(
        GetProductionStatisticsDto dto)
    {
        return await _productionRecordService.GetProductionStatisticsAsync(dto);
    }
}
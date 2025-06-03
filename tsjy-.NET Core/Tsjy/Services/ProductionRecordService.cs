using Faoem.Common.Dtos;
using Faoem.Facility.Services.Facility;
using Faoem.Variable.EventArgs;
using Faoem.Variable.Services.Variable;
using Microsoft.EntityFrameworkCore;
using Tsjy.DbContexts;
using Tsjy.Definitions;
using Tsjy.Dtos;
using Tsjy.Eunms;
using Tsjy.Models;

namespace Tsjy.Services;

public class ProductionRecordService
{
    // Singleton

    private readonly IVariableService _variableService;
    private readonly ProductionBindingService _productionBindingService;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly CraftRecordService _craftRecordService;

    // <facilityId, productionData>
    private Dictionary<long, ProductionData> _productionDataDict = [];

    public ProductionRecordService(
        IVariableService variableService,
        ProductionBindingService productionBindingService,
        IServiceScopeFactory serviceScopeFactory,
        CraftRecordService craftRecordService
    )
    {
        _variableService = variableService;
        _productionBindingService = productionBindingService;
        _serviceScopeFactory = serviceScopeFactory;
        _craftRecordService = craftRecordService;

        _variableService.VariableChangedAsync += VariableServiceOnVariableChangedAsync;
    }

    private async Task VariableServiceOnVariableChangedAsync(VariableChangedEventArgs arg)
    {
        var bindings = await _productionBindingService.GetProductionBindingsAsync();
        var variables = arg.Variables;

        // Check if the variable is bound to production
        foreach (var variable in variables)
        {
            var binding = bindings.FirstOrDefault(productionBinding =>
                productionBinding.ConnectorInstance == variable.ConnectorInstance &&
                productionBinding.ConnectionName == variable.ConnectionName &&
                productionBinding.DataPoint == variable.DataPointName &&
                productionBinding.Name == variable.Name);
            if (binding is null)
            {
                // Not a variable bound to production
                continue;
            }

            // Get the production data
            if (!_productionDataDict.TryGetValue(binding.FacilityId, out var productionData))
            {
                productionData = new ProductionData();
                _productionDataDict.Add(binding.FacilityId, productionData);
            }

            // Update the production data
            switch (binding.BindingType)
            {
                case ProductionBinding.CustomerCode:
                    if (variable.Val?.ToString() is string customerCode)
                    {
                        productionData.CustomerCode = customerCode;
                    }

                    break;
                case ProductionBinding.MaterialSpecification:
                    if (variable.Val?.ToString() is string materialSpecification)
                    {
                        productionData.MaterialSpecification = materialSpecification;
                    }

                    break;
                case ProductionBinding.StartSignal:
                    if (bool.TryParse(variable.Val?.ToString(), out bool startSignal))
                    {
                        if (productionData.StartSignal == false && startSignal)
                        {
                            // F->T 记录工艺数据
                            await _craftRecordService.SaveCraftDataAsync(productionData.CustomerCode,
                                productionData.MaterialSpecification);
                        }

                        productionData.StartSignal = startSignal;
                    }

                    break;
                case ProductionBinding.CompleteSignal:
                    // F->T 记录切割完成
                    if (bool.TryParse(variable.Val?.ToString(), out bool completeSignal))
                    {
                        if (productionData.CompleteSignal == false && completeSignal)
                        {
                            // F->T 记录切割完成
                            using var scope = _serviceScopeFactory.CreateScope();
                            var dbContext = scope.ServiceProvider.GetRequiredService<TsjyDbContext>();
                            await dbContext.ProductionData.AddAsync(new TsjyProductionData
                            {
                                FacilityId = binding.FacilityId,
                                CustomerCode = productionData.CustomerCode,
                                MaterialSpecification = productionData.MaterialSpecification,
                                CompleteTime = DateTimeOffset.Now.ToUnixTimeSeconds()
                            });

                            await dbContext.SaveChangesAsync();
                        }

                        productionData.CompleteSignal = completeSignal;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public async Task<PagedDto<TsjyProductionData>> GetProductionHistoryAsync(GetProductionHistoryDto dto)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var sp = scope.ServiceProvider;
        var tsjyDbContext = sp.GetRequiredService<TsjyDbContext>();
        var query = tsjyDbContext.ProductionData
            .Where(data => data.FacilityId == dto.FacilityId)
            .Where(data => data.CompleteTime >= dto.StartTime && data.CompleteTime <= dto.EndTime);
        var total = await query.CountAsync();
        var items = await query
            .Skip((dto.PageIndex - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .ToListAsync();

        if (items.Count > 0)
        {
            var facilityService = sp.GetRequiredService<IFacilityService>();
            foreach (var item in items)
            {
                item.Facility = await facilityService.GetFacilityAsync(item.FacilityId);
            }
        }

        return new PagedDto<TsjyProductionData>
        {
            Total = total,
            Items = items.OrderByDescending(x => x.CompleteTime).ToList()
        };
    }

    public async Task<List<ProductionStatisticsDto>> GetProductionStatisticsAsync(GetProductionStatisticsDto dto)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var sp = scope.ServiceProvider;
        var tsjyDbContext = sp.GetRequiredService<TsjyDbContext>();
        var query = tsjyDbContext.ProductionData
            .Where(data => data.FacilityId == dto.FacilityId)
            .Where(data => data.CompleteTime >= dto.StartTime && data.CompleteTime <= dto.EndTime);

        var customers = await query
            .Select(data => data.CustomerCode)
            .Distinct()
            .ToListAsync();

        List<ProductionStatisticsDto> statistics = [];
        foreach (var customer in customers)
        {
            var specifications = await query
                .Where(data => data.CustomerCode == customer)
                .Select(data => data.MaterialSpecification)
                .Distinct()
                .ToListAsync();
            List<MaterialSpecificationDto> specs = [];
            foreach (var specification in specifications)
            {
                var specCount = await query
                    .Where(data => data.CustomerCode == customer)
                    .Where(data => data.MaterialSpecification == specification)
                    .CountAsync();
                specs.Add(new MaterialSpecificationDto()
                {
                    Name = specification,
                    Count = specCount
                });
            }

            var total = specs.Select(specificationDto => specificationDto.Count)
                .Sum();
            statistics.Add(new ProductionStatisticsDto
            {
                CustomerCode = customer,
                TotalCount = total,
                MaterialSpecifications = specs.OrderByDescending(specificationDto => specificationDto.Count).ToList()
            });
        }

        return statistics.OrderByDescending(statisticsDto => statisticsDto.TotalCount).ToList();
    }

    public async Task CleanupHistoryAsync()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var tsjyDbContext = scope.ServiceProvider.GetRequiredService<TsjyDbContext>();
        var now = DateTimeOffset.Now.ToUnixTimeSeconds();
        var query = tsjyDbContext.ProductionData
            .Where(data => now - data.CompleteTime > 30 * 24 * 60 * 60);
        tsjyDbContext.ProductionData.RemoveRange(query);
        await tsjyDbContext.SaveChangesAsync();
    }
}
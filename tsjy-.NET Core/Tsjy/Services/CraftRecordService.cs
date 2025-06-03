using Faoem.Common.Dtos;
using Faoem.S7Connector.Definitions;
using Faoem.Variable.EventArgs;
using Faoem.Variable.Services.Variable;
using Microsoft.EntityFrameworkCore;
using Tsjy.DbContexts;
using Tsjy.Dtos;
using Tsjy.Eunms;
using Tsjy.Models;
using Data = Faoem.OpcUaConnector.Definitions.Data;

namespace Tsjy.Services;

public class CraftRecordService
{
    // singleton

    private readonly IVariableService _variableService;
    private readonly CraftBindingService _craftBindingService;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    // <facilityId, data>
    private readonly Dictionary<long, List<TsjyCraftData>> _craftDataDict = [];

    public CraftRecordService(
        IVariableService variableService,
        CraftBindingService craftBindingService,
        IServiceScopeFactory serviceScopeFactory
    )
    {
        _variableService = variableService;
        _craftBindingService = craftBindingService;
        _serviceScopeFactory = serviceScopeFactory;

        _variableService.VariableChangedAsync += VariableServiceOnVariableChangedAsync;
    }

    private async Task VariableServiceOnVariableChangedAsync(VariableChangedEventArgs arg)
    {
        var variables = arg.Variables;
        var bindings = await _craftBindingService.GetCraftBindingsAsync();

        foreach (var variable in variables)
        {
            var binding = bindings.FirstOrDefault(craftBinding =>
                craftBinding.ConnectorInstance == variable.ConnectorInstance &&
                craftBinding.ConnectionName == variable.ConnectionName &&
                craftBinding.DataPoint == variable.DataPointName &&
                craftBinding.Name == variable.Name);
            if (binding is null)
            {
                continue;
            }

            var parseOk = double.TryParse(variable.Val?.ToString(), out double value);
            if (!parseOk)
            {
                continue;
            }

            var craftData = new TsjyCraftData()
            {
                BindingType = binding.BindingType,
                FacilityId = binding.FacilityId,
                Index = binding.Index,
                Value = value
            };

            if (!_craftDataDict.TryGetValue(binding.FacilityId, out var craftDataList))
            {
                // No data for this facility
                craftDataList =
                [
                    craftData
                ];
                _craftDataDict.Add(binding.FacilityId, craftDataList);
            }
            else
            {
                // Data exists for this facility
                var existingData = craftDataList.FirstOrDefault(data =>
                    data.BindingType == craftData.BindingType &&
                    data.Index == craftData.Index);
                if (existingData is null)
                {
                    // No data for this binding type and index
                    craftDataList.Add(craftData);
                }
                else
                {
                    existingData.Value = value;
                }
            }
        }
    }

    public async Task SaveCraftDataAsync(string customerCode, string materialSpecification)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TsjyDbContext>();

        // 使每次保存行为的时间一致
        var now = DateTimeOffset.Now.ToUnixTimeSeconds();

        foreach (var (_, craftDataList) in _craftDataDict)
        {
            foreach (var craftData in craftDataList)
            {
                await dbContext.CraftData.AddAsync(new TsjyCraftData()
                {
                    BindingType = craftData.BindingType,
                    FacilityId = craftData.FacilityId,
                    Index = craftData.Index,
                    Value = craftData.Value,
                    Time = now,
                    CustomerCode = customerCode,
                    MaterialSpecification = materialSpecification
                });
            }
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task<PagedDto<CraftDataDto>> GetCraftDataAsync(GetCraftDataDto dto)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TsjyDbContext>();
        var q = dbContext.CraftData.Where(data => data.Time >= dto.StartTime && data.Time <= dto.EndTime)
            .Where(data => data.FacilityId == dto.FacilityId);

        // 去重后的时间
        var times = q.Select(data => data.Time)
            .Distinct()
            .OrderByDescending(time => time);

        // 分页后的时间
        var pagedTimes = times.Skip((dto.PageIndex - 1) * dto.PageSize)
            .Take(dto.PageSize);

        // 所有符合的数据，进行本地查询，避免多次查询数据库
        var originalData = await q.Where(data => times.Contains(data.Time))
            .ToListAsync();

        var dtoId = 0;
        var types = Enum.GetValues<CraftBinding>().ToList();
        var items = new List<CraftDataDto>();
        foreach (var time in pagedTimes)
        {
            var indexes = originalData.Where(data => data.Time == time)
                .Select(data => data.Index)
                .Distinct()
                .ToList();
            if (indexes.Count == 0)
            {
                continue;
            }

            // 前面已经筛选过，理论上这里至少有一个
            var firstHistory = originalData.First(data => data.Time == time);
            var craftDto = new CraftDataDto()
            {
                // 使用负数避免和真实的 id 冲突
                Id = --dtoId,
                Time = time,
                CustomerCode = firstHistory.CustomerCode,
                MaterialSpecification = firstHistory.MaterialSpecification
            };

            foreach (var index in indexes)
            {
                var history = new CraftHistoryDto()
                {
                    Index = index
                };
                foreach (var type in types)
                {
                    var historyData = originalData.Where(data => data.Time == time)
                        .Where(data => data.Index == index)
                        .FirstOrDefault(data => data.BindingType == type);
                    switch (type)
                    {
                        case CraftBinding.SectionHeight:
                            history.SectionHeight = historyData?.Value ?? 0;
                            break;
                        case CraftBinding.InfeedVelocity:
                            history.InfeedVelocity = historyData?.Value ?? 0;
                            break;
                        case CraftBinding.NewLineSpeed:
                            history.NewLineSpeed = historyData?.Value ?? 0;
                            break;
                        case CraftBinding.LineVelocity:
                            history.LineVelocity = historyData?.Value ?? 0;
                            break;
                    }
                }

                craftDto.Children.Add(history);
            }

            items.Add(craftDto);
        }

        return new PagedDto<CraftDataDto>()
        {
            Items = items,
            Total = times.Count()
        };
    }

    public async Task CleanupHistoryAsync()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var tsjyDbContext = scope.ServiceProvider.GetRequiredService<TsjyDbContext>();
        var now = DateTimeOffset.Now.ToUnixTimeSeconds();
        var q = tsjyDbContext.CraftData
            .Where(data => now - data.Time > 30 * 24 * 60 * 60);
        tsjyDbContext.CraftData.RemoveRange(q);
        await tsjyDbContext.SaveChangesAsync();
    }
}
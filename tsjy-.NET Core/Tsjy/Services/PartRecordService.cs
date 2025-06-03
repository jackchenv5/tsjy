using Faoem.Common.Dtos;
using Faoem.Common.Exceptions;
using Faoem.Variable.EventArgs;
using Faoem.Variable.Services.Variable;
using Microsoft.EntityFrameworkCore;
using Tsjy.DbContexts;
using Tsjy.Dtos;
using Tsjy.Models;

namespace Tsjy.Services;

public class PartRecordService
{
    // singleton

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IVariableService _variableService;

    // <facilityId, data>
    private List<TsjyPart> _parts = [];

    public PartRecordService(
        IServiceScopeFactory serviceScopeFactory,
        IVariableService variableService
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _variableService = variableService;

        RefreshParts();

        _variableService.VariableChangedAsync += VariableServiceOnVariableChangedAsync;
    }

    private async Task VariableServiceOnVariableChangedAsync(VariableChangedEventArgs arg)
    {
        var variables = arg.Variables;
        if (variables.Count == 0)
        {
            return;
        }

        var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TsjyDbContext>();
        foreach (var variable in variables)
        {
            var partWithBinding = _parts.FirstOrDefault(part =>
                part.ConnectorInstance == variable.ConnectorInstance &&
                part.ConnectionName == variable.ConnectionName &&
                part.DataPoint == variable.DataPointName &&
                part.VariableName == variable.Name);
            if (partWithBinding is null)
            {
                continue;
            }

            var parseOk = double.TryParse(variable.Val?.ToString(), out double value);
            if (!parseOk)
            {
                continue;
            }

            partWithBinding.LastValue = value;
            partWithBinding.UpdatedAt = DateTimeOffset.Now.ToUnixTimeSeconds();
            dbContext.Parts.Update(partWithBinding);

            var roundValue = Math.Round(value, 15);
            var now = DateTimeOffset.Now.ToUnixTimeSeconds();
            if (roundValue <= 0.0)
            {
                // 寿命为 0 时进行记录
                // 为了避免出现浮点精度问题，取小数点后 16 位
                var history = new TsjyPartMaintainHistory()
                {
                    PartName = partWithBinding.PartName,
                    Remark = partWithBinding.Remark,
                    FacilityId = partWithBinding.FacilityId,
                    ProcessTime = now,
                    Time = now,
                    Reason = "计划维护",
                    ProcessMethod = "计划维护"
                };
                await dbContext.PartMaintainHistories.AddAsync(history);
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private void RefreshParts()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TsjyDbContext>();
        _parts = dbContext.Parts.ToList();
    }

    public async Task AddPartAsync(List<TsjyPart> parts)
    {
        if (parts.Count == 0)
        {
            throw new AppException("Parts is empty.", 400);
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TsjyDbContext>();
        foreach (var part in parts)
        {
            var exist = await dbContext.Parts
                .FirstOrDefaultAsync(tsjyPart => tsjyPart.FacilityId == part.FacilityId &&
                                                 tsjyPart.PartName == part.PartName);
            if (exist is not null)
            {
                throw new AppException("Part already exists.", 400);
            }

            await dbContext.Parts.AddAsync(part);
        }

        await dbContext.SaveChangesAsync();
        RefreshParts();
    }

    public async Task UpdatePartAsync(TsjyPart part)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TsjyDbContext>();
        var exist = await dbContext.Parts.FindAsync(part.Id);
        if (exist is null)
        {
            throw new AppException("Part does not exist.", 400);
        }

        exist.PartName = part.PartName;
        exist.Remark = part.Remark;
        exist.ConnectorInstance = part.ConnectorInstance;
        exist.ConnectionName = part.ConnectionName;
        exist.DataPoint = part.DataPoint;
        exist.VariableName = part.VariableName;

        await dbContext.SaveChangesAsync();
        RefreshParts();
    }

    public async Task DeletePartAsync(long id)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TsjyDbContext>();
        var exist = await dbContext.Parts
            .FirstOrDefaultAsync(tsjyPart => tsjyPart.Id == id);
        if (exist is null)
        {
            throw new AppException("Part does not exist.", 400);
        }

        dbContext.Parts.Remove(exist);
        await dbContext.SaveChangesAsync();
        RefreshParts();
    }

    public async Task<List<TsjyPart>> GetPartsAsync(long facilityId)
    {
        return await Task.FromResult
        (
            _parts.Where(part => part.FacilityId == facilityId).ToList()
        );
    }

    public async Task<PagedDto<TsjyPartMaintainHistory>> GetPartMaintainHistoriesAsync(GetPartMaintainHistoryDto dto)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TsjyDbContext>();
        var query = dbContext.PartMaintainHistories
            .Where(history => history.ProcessTime >= dto.StartTime && history.ProcessTime <= dto.EndTime);
        if (dto.FacilityId != 0)
        {
            query = query.Where(history => history.FacilityId == dto.FacilityId);
        }

        var total = await query.CountAsync();
        var histories = await query
            .OrderByDescending(history => history.Time)
            .Skip((dto.PageIndex - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .ToListAsync();

        return new PagedDto<TsjyPartMaintainHistory>
        {
            Total = total,
            Items = histories
        };
    }

    public async Task AddPartMaintainHistoryAsync(TsjyPartMaintainHistory dto)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TsjyDbContext>();
        dto.Time = DateTimeOffset.Now.ToUnixTimeSeconds();
        await dbContext.AddAsync(dto);
        await dbContext.SaveChangesAsync();
    }

    public async Task CleanupHistoryAsync()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var tsjyDbContext = scope.ServiceProvider.GetRequiredService<TsjyDbContext>();
        var now = DateTimeOffset.Now.ToUnixTimeSeconds();
        var q = tsjyDbContext.PartMaintainHistories
            .Where(history => now - history.Time > 30 * 24 * 60 * 60);
        tsjyDbContext.PartMaintainHistories.RemoveRange(q);
        await tsjyDbContext.SaveChangesAsync();
    }
}
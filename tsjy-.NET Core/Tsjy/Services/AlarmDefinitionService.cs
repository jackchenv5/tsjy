using Faoem.Common.Dtos;
using Faoem.Common.Exceptions;
using Faoem.Facility.Services.Facility;
using Microsoft.EntityFrameworkCore;
using Tsjy.DbContexts;
using Tsjy.Models;

namespace Tsjy.Services;

public class AlarmDefinitionService(
    TsjyDbContext tsjyDbContext,
    IFacilityService facilityService,
    AlarmRecordService alarmRecordService
)
{
    public async Task<PagedDto<TsjyAlarmDefinition>> GetAlarmDefinitionsAsync(int pageIndex = 1, int pageSize = 20)
    {
        var total = await tsjyDbContext.AlarmDefinitions.CountAsync();
        var items = await tsjyDbContext.AlarmDefinitions
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        foreach (var definition in items)
        {
            definition.Facility = await facilityService.GetFacilityAsync(definition.FacilityId);
        }

        return new PagedDto<TsjyAlarmDefinition>()
        {
            Total = total,
            Items = items
        };
    }

    public async Task<TsjyAlarmDefinition> AddAlarmDefinitionAsync(TsjyAlarmDefinition definition)
    {
        var entry = await tsjyDbContext.AlarmDefinitions.AddAsync(definition);
        await tsjyDbContext.SaveChangesAsync();
        await alarmRecordService.RefreshAlarmDefinitionsAsync();
        return entry.Entity;
    }

    public async Task AddAlarmDefinitionsAsync(List<TsjyAlarmDefinition> definitions)
    {
        foreach (var definition in definitions)
        {
            var exist = await tsjyDbContext.AlarmDefinitions.AnyAsync(alarmDefinition =>
                alarmDefinition.FacilityId == definition.FacilityId &&
                alarmDefinition.ConnectorInstance == definition.ConnectorInstance &&
                alarmDefinition.ConnectionName == definition.ConnectionName &&
                alarmDefinition.DataPoint == definition.DataPoint &&
                alarmDefinition.Name == definition.Name &&
                alarmDefinition.TriggerType == definition.TriggerType
            );
            if (exist)
            {
                continue;
            }

            await tsjyDbContext.AlarmDefinitions.AddAsync(definition);
        }

        await tsjyDbContext.SaveChangesAsync();
        await alarmRecordService.RefreshAlarmDefinitionsAsync();
    }

    public async Task UpdateAlarmDefinitionAsync(TsjyAlarmDefinition definition)
    {
        var entity = await tsjyDbContext.AlarmDefinitions.FindAsync(definition.Id);
        if (entity is null)
        {
            throw new AppException("Alarm definition not found", 404);
        }

        tsjyDbContext.Entry(entity).CurrentValues.SetValues(definition);
        await tsjyDbContext.SaveChangesAsync();
        await alarmRecordService.RefreshAlarmDefinitionsAsync();
    }

    public async Task DeleteAlarmDefinitionAsync(long definitionId)
    {
        var entity = await tsjyDbContext.AlarmDefinitions.FindAsync(definitionId);
        if (entity is null)
        {
            throw new AppException("Alarm definition not found", 404);
        }

        tsjyDbContext.AlarmDefinitions.Remove(entity);
        await tsjyDbContext.SaveChangesAsync();
        await alarmRecordService.RefreshAlarmDefinitionsAsync();
    }
}
using Faoem.Common.Dtos;
using Faoem.Facility.Services.Facility;
using Microsoft.EntityFrameworkCore;
using Tsjy.DbContexts;
using Tsjy.Dtos;
using Tsjy.Models;

namespace Tsjy.Services;

public class AlarmService(
    TsjyDbContext tsjyDbContext,
    IFacilityService facilityService
)
{
    public async Task<List<TsjyAlarmHistory>> GetCurrentAlarmsAsync()
    {
        var alarms = await tsjyDbContext.AlarmHistories
            .Where(history => history.EndTime == null)
            .ToListAsync();

        foreach (var alarm in alarms)
        {
            alarm.Facility = await facilityService.GetFacilityAsync(alarm.FacilityId);
        }

        return alarms;
    }

    public async Task<PagedDto<TsjyAlarmHistory>> GetAlarmHistoryAsync(GetAlarmHistoryDto param)
    {
        var query = tsjyDbContext.AlarmHistories
            .Where(history => history.StartTime >= param.StartTime && history.EndTime <= param.EndTime);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(history => history.Id)
            .Skip((param.PageIndex - 1) * param.PageSize)
            .Take(param.PageSize)
            .ToListAsync();

        foreach (var item in items)
        {
            item.Facility = await facilityService.GetFacilityAsync(item.FacilityId);
        }

        return new PagedDto<TsjyAlarmHistory>
        {
            Total = total,
            Items = items
        };
    }

    public async Task<List<AlarmCountDto>> GetAlarmCountAsync(GetAlarmHistoryDto param)
    {
        var query = tsjyDbContext.AlarmHistories
            .Where(history => history.StartTime >= param.StartTime && history.EndTime <= param.EndTime)
            .GroupBy(history => history.AlarmDefinitionId)
            .Select(group => new
            {
                AlarmDefinitionId = group.Key,
                Count = group.Count()
            })
            .OrderByDescending(item => item.Count);

        var top = await query.Take(5).ToListAsync();
        var other = await query.Skip(5).SumAsync(item => item.Count);

        List<AlarmCountDto> result = [];
        foreach (var g in top)
        {
            var definition = await tsjyDbContext.AlarmDefinitions.FindAsync(g.AlarmDefinitionId);
            if (definition is null)
            {
                continue;
            }

            result.Add(new AlarmCountDto
            {
                Message = definition.Message,
                Count = g.Count
            });
        }

        result.Add(new AlarmCountDto
        {
            Message = string.Empty,
            Count = other
        });

        return result;
    }
}
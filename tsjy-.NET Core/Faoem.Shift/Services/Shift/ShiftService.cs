using Faoem.Common.Exceptions;
using Faoem.Shift.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Faoem.Shift.Services.Shift;

internal class ShiftService(ShiftDbContext shiftDbContext) : IShiftService
{
    public async Task<Models.Shift?> GetCurrentShiftAsync()
    {
        var utcNow = DateTimeOffset.UtcNow;
        var shiftList = await shiftDbContext.Shifts.ToListAsync();
        long fixedStartTime = 0;
        long fixedEndTime = 0;
        var currentShift = shiftList.FirstOrDefault(shift =>
            {
                var shiftStartTime = DateTimeOffset.FromUnixTimeSeconds(shift.StartTime);
                var shiftEndTime = DateTimeOffset.FromUnixTimeSeconds(shift.EndTime);
                var startTime = new DateTimeOffset(
                    utcNow.Year,
                    utcNow.Month,
                    utcNow.Day,
                    shiftStartTime.Hour,
                    shiftStartTime.Minute,
                    shiftStartTime.Second,
                    utcNow.Offset
                );
                var endTime = new DateTimeOffset(
                    utcNow.Year,
                    utcNow.Month,
                    utcNow.Day,
                    shiftEndTime.Hour,
                    shiftEndTime.Minute,
                    shiftEndTime.Second,
                    utcNow.Offset
                );

                if (startTime > endTime)
                {
                    // 不在同一天
                    if (utcNow > startTime)
                    {
                        endTime = endTime.AddDays(1);
                    }

                    if (utcNow < startTime)
                    {
                        startTime = startTime.AddDays(-1);
                    }
                }

                var isCurrentShift = utcNow >= startTime && utcNow < endTime;
                if (!isCurrentShift)
                {
                    return isCurrentShift;
                }

                fixedStartTime = startTime.ToUnixTimeSeconds();
                fixedEndTime = endTime.ToUnixTimeSeconds();

                return isCurrentShift;
            }
        );

        if (currentShift is null)
        {
            return currentShift;
        }

        currentShift.StartTime = fixedStartTime;
        currentShift.EndTime = fixedEndTime;

        return currentShift;
    }

    public async Task<List<Models.Shift>> GetShiftsAsync()
    {
        var shifts = await shiftDbContext.Shifts.ToListAsync();
        var utcNow = DateTimeOffset.UtcNow;

        foreach (var shift in shifts)
        {
            var shiftStartTime = DateTimeOffset.FromUnixTimeSeconds(shift.StartTime);
            var shiftEndTime = DateTimeOffset.FromUnixTimeSeconds(shift.EndTime);
            var startTime = new DateTimeOffset(
                utcNow.Year,
                utcNow.Month,
                utcNow.Day,
                shiftStartTime.Hour,
                shiftStartTime.Minute,
                shiftStartTime.Second,
                utcNow.Offset
            );
            var endTime = new DateTimeOffset(
                utcNow.Year,
                utcNow.Month,
                utcNow.Day,
                shiftEndTime.Hour,
                shiftEndTime.Minute,
                shiftEndTime.Second,
                utcNow.Offset
            );

            if (!shift.SpanTheDay)
            {
                shift.StartTime = startTime.ToUnixTimeSeconds();
                shift.EndTime = endTime.ToUnixTimeSeconds();
            }
            else
            {
                if (utcNow >= startTime)
                {
                    endTime = endTime.AddDays(1);
                }
                else
                {
                    startTime = startTime.AddDays(-1);
                }

                shift.StartTime = startTime.ToUnixTimeSeconds();
                shift.EndTime = endTime.ToUnixTimeSeconds();
            }
        }

        return shifts;
    }

    public async Task UpdateShiftsAsync(List<Models.Shift> shifts)
    {
        var exist = shifts.Where(shift => shiftDbContext.Shifts.Any(s => s.Id == shift.Id));
        if (!exist.Count().Equals(shifts.Count))
        {
            throw new AppException("Some shifts do not exist.", 400);
        }

        shiftDbContext.Shifts.UpdateRange(shifts);
        await shiftDbContext.SaveChangesAsync();
    }
}
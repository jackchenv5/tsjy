using Faoem.Common.Dtos;
using Faoem.Common.Exceptions;
using Faoem.FacilityStatus.Dtos;
using Faoem.FacilityStatus.Inputs;
using Faoem.Shift.Services.Shift;
using Faoem.Variable.Services.InfluxDbClient;

namespace Faoem.FacilityStatus.Services.FacilityStatus;

internal class FacilityStatusService(
    IShiftService shiftService,
    IInfluxDbClientService influxDbClientService
) : IFacilityStatusService
{
    public async Task<StatusDto> GetCurrentShiftStatusAsync(long facilityId)
    {
        var currentShift = await shiftService.GetCurrentShiftAsync();
        if (currentShift is null)
        {
            throw new AppException("There is no shift at the time.", statusCode: 400);
        }

        var shiftStartTime = currentShift.StartTime;
        var shiftEndTime = currentShift.EndTime;

        var records = await GetAllStatusAsync(new StatusQueryInput
        {
            StartTime = shiftStartTime,
            EndTime = shiftEndTime,
            FacilityId = facilityId
        });
        if (records.Count == 0)
        {
            return new StatusDto
            {
                CurrentStatus = Status.Invalid
            };
        }

        var runningSeconds = records
            .Where(record => record.Status == "Running")
            .Sum(record => record.Duration);
        var standbySeconds = records
            .Where(record => record.Status == "Standby")
            .Sum(record => record.Duration);
        var stoppedSeconds = records
            .Where(record => record.Status == "Stopped")
            .Sum(record => record.Duration);
        var errorSeconds = records
            .Where(record => record.Status == "Error")
            .Sum(record => record.Duration);

        var statusDto = new StatusDto();
        statusDto.RunningSeconds = runningSeconds;
        statusDto.StandbySeconds = standbySeconds;
        statusDto.StoppedSeconds = stoppedSeconds;
        statusDto.ErrorSeconds = errorSeconds;
        statusDto.CurrentStatus = Enum.TryParse(records[0].Status, out Status currentStatus)
            ? currentStatus
            : Status.Invalid;

        return statusDto;
    }

    public async Task<PagedDto<StatusRecordDto>> GetStatusAsync(StatusQueryInput statusQuery)
    {
        var pageIndex = statusQuery.PageIndex;
        var pageSize = statusQuery.PageSize;

        var records = await GetAllStatusAsync(statusQuery);

        return new PagedDto<StatusRecordDto>
        {
            Total = records.Count,
            Items = records.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
        };
    }

    public async Task<List<StatusRecordDto>> GetAllStatusAsync(StatusQueryInput statusQuery)
    {
        var facilityId = statusQuery.FacilityId;
        var startTime = statusQuery.StartTime;
        var endTime = statusQuery.EndTime;

        // 将 startTime 和 endTime 转换为 "yyyy-MM-ddTHH:mm:ssZ" 格式
        var startTimeStr = DateTimeOffset.FromUnixTimeSeconds(startTime).ToString("yyyy-MM-ddTHH:mm:ssZ");
        var endTimeStr = DateTimeOffset.FromUnixTimeSeconds(endTime).ToString("yyyy-MM-ddTHH:mm:ssZ");

        var tags = new List<(string, string)>
        {
            ("facilityId", $"{facilityId}"),
        };

        var fluxTables = await influxDbClientService.QueryDataAsync(tags, startTimeStr, endTimeStr);

        var fluxRecords = fluxTables
            .SelectMany(table => table.Records)
            .Select(fluxRecord =>
                {
                    var time = fluxRecord.GetTime()?.ToUnixTimeSeconds();
                    var status = (string)fluxRecord.GetValueByKey("_field");
                    var shiftId = Convert.ToInt32(fluxRecord.GetValueByKey("shiftId"));

                    return new
                    {
                        Time = time!.Value,
                        Status = status,
                        ShiftId = shiftId
                    };
                }
            )
            .OrderByDescending(item => item.Time)
            .ToList();

        var shifts = await shiftService.GetShiftsAsync();

        var statusRecords = new List<StatusRecordDto>();

        for (var i = 0; i < fluxRecords.Count; i++)
        {
            var shift = shifts.FirstOrDefault(s => s.Id == fluxRecords[i].ShiftId);
            if (shift is null)
            {
                continue;
            }

            var time = fluxRecords[i].Time;
            var status = fluxRecords[i].Status;
            long duration;
            // 已经按时间倒序
            if (i == 0)
            {
                duration = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - time;
            }
            else
            {
                duration = fluxRecords[i - 1].Time - time;
            }

            var statusRecord = new StatusRecordDto(time, status, shift, (int)duration);
            statusRecords.Add(statusRecord);
        }

        return statusRecords;
    }
}
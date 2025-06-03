using Quartz;
using Tsjy.Services;

namespace Tsjy.Jobs;

public class MotorJob : IJob
{
    public static readonly string JobKey = "MotorJob";

    private readonly MotorRecordService _motorRecordService;

    public MotorJob(MotorRecordService motorRecordService)
    {
        _motorRecordService = motorRecordService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _motorRecordService.ArchiveMotorDataAsync();
    }
}
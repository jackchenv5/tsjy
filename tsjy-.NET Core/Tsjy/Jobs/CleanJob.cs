using Quartz;
using Tsjy.Services;

namespace Tsjy.Jobs;

public class CleanJob : IJob
{
    public static readonly string JobKey = "CleanJob";

    private readonly AlarmRecordService _alarmRecordService;
    private readonly ProductionRecordService _productionRecordService;
    private readonly PartRecordService _partRecordService;
    private readonly CraftRecordService _craftRecordService;

    public CleanJob(
        AlarmRecordService alarmRecordService,
        ProductionRecordService productionRecordService,
        PartRecordService partRecordService,
        CraftRecordService craftRecordService
    )
    {
        _alarmRecordService = alarmRecordService;
        _productionRecordService = productionRecordService;
        _partRecordService = partRecordService;
        _craftRecordService = craftRecordService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _alarmRecordService.CleanupHistoryAsync();
        await _productionRecordService.CleanupHistoryAsync();
        await _partRecordService.CleanupHistoryAsync();
        await _craftRecordService.CleanupHistoryAsync();
    }
}
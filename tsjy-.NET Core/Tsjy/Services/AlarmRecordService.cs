using Faoem.Variable.EventArgs;
using Faoem.Variable.Services.Variable;
using Microsoft.EntityFrameworkCore;
using Tsjy.DbContexts;
using Tsjy.Models;

namespace Tsjy.Services;

public class AlarmRecordService
{
    private readonly IVariableService _variableService;
    private readonly ILogger<StatusRecordService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AlarmRecordService(
        IVariableService variableService,
        ILogger<StatusRecordService> logger,
        IServiceScopeFactory serviceScopeFactory
    )
    {
        _variableService = variableService;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;

        RefreshAlarmDefinitionsAsync().Wait();

        _variableService.VariableChangedAsync += VariableServiceOnVariableChangedAsync;
    }

    // <报警定义 id, 报警历史记录 id>
    // 报警历史记录 id 用于结束报警
    private readonly Dictionary<long, long> _currentAlarmDict = new();

    private async Task VariableServiceOnVariableChangedAsync(VariableChangedEventArgs arg)
    {
        var variables = arg.Variables;

        foreach (var variable in variables)
        {
            var alarmDefinition = _alarmDefinitions.FirstOrDefault(definition =>
                definition.ConnectorInstance == variable.ConnectorInstance &&
                definition.ConnectionName == variable.ConnectionName &&
                definition.DataPoint == variable.DataPointName &&
                definition.Name == variable.Name);
            if (alarmDefinition is null)
            {
                // 不是报警定义绑定的变量
                continue;
            }

            // 处理报警
            var dataType = alarmDefinition.DataType;
            var triggerType = alarmDefinition.TriggerType;
            if (dataType == "int")
            {
                if (!long.TryParse(alarmDefinition.TriggerValue, out var triggerValue))
                {
                    // 无法转换为指定的数据类型，跳过
                    _logger.LogError("Trigger value is not a valid number: {TriggerValue}",
                        alarmDefinition.TriggerValue);
                    continue;
                }

                long val;
                if (variable.DataType == "Bool")
                {
                    val = (bool)(variable.Val ?? false) ? 1 : 0;
                }
                else
                {
                    val = (long)(variable.Val ?? 0);
                }


                if (triggerType == TriggerType.GreaterThan)
                {
                    if (val > triggerValue)
                    {
                        // 触发报警
                        await AlarmStartAsync(alarmDefinition);
                    }
                    else
                    {
                        await AlarmEndAsync(alarmDefinition);
                    }
                }

                if (triggerType == TriggerType.LessThan)
                {
                    if (val < triggerValue)
                    {
                        // 触发报警
                        await AlarmStartAsync(alarmDefinition);
                    }
                    else
                    {
                        await AlarmEndAsync(alarmDefinition);
                    }
                }

                if (triggerType == TriggerType.Equal)
                {
                    if (val == triggerValue)
                    {
                        // 触发报警
                        await AlarmStartAsync(alarmDefinition);
                    }
                    else
                    {
                        await AlarmEndAsync(alarmDefinition);
                    }
                }

                if (triggerType == TriggerType.NotEqual)
                {
                    if (val != triggerValue)
                    {
                        // 触发报警
                        await AlarmStartAsync(alarmDefinition);
                    }
                    else
                    {
                        await AlarmEndAsync(alarmDefinition);
                    }
                }

                if (triggerType == TriggerType.GreaterThanOrEqual)
                {
                    if (val >= triggerValue)
                    {
                        // 触发报警
                        await AlarmStartAsync(alarmDefinition);
                    }
                    else
                    {
                        await AlarmEndAsync(alarmDefinition);
                    }
                }

                if (triggerType == TriggerType.LessThanOrEqual)
                {
                    if (val >= triggerValue)
                    {
                        // 触发报警
                        await AlarmStartAsync(alarmDefinition);
                    }
                    else
                    {
                        await AlarmEndAsync(alarmDefinition);
                    }
                }
            }
            else if (dataType == "float")
            {
                if (!double.TryParse(alarmDefinition.TriggerValue, out var triggerValue))
                {
                    // 无法转换为指定的数据类型，跳过
                    _logger.LogError("Trigger value is not a valid number: {TriggerValue}",
                        alarmDefinition.TriggerValue);
                    continue;
                }

                var val = (double)(variable.Val ?? 0.0);
                if (triggerType == TriggerType.GreaterThan)
                {
                    if (val > triggerValue)
                    {
                        // 触发报警
                        await AlarmStartAsync(alarmDefinition);
                    }
                    else
                    {
                        await AlarmEndAsync(alarmDefinition);
                    }
                }

                if (triggerType == TriggerType.LessThan)
                {
                    if (val < triggerValue)
                    {
                        // 触发报警
                        await AlarmStartAsync(alarmDefinition);
                    }
                    else
                    {
                        await AlarmEndAsync(alarmDefinition);
                    }
                }

                if (triggerType == TriggerType.Equal)
                {
                    if (Math.Abs(val - triggerValue) < 0.000001)
                    {
                        // 触发报警
                        await AlarmStartAsync(alarmDefinition);
                    }
                    else
                    {
                        await AlarmEndAsync(alarmDefinition);
                    }
                }

                if (triggerType == TriggerType.NotEqual)
                {
                    if (Math.Abs(val - triggerValue) > 0.000001)
                    {
                        // 触发报警
                        await AlarmStartAsync(alarmDefinition);
                    }
                    else
                    {
                        await AlarmEndAsync(alarmDefinition);
                    }
                }

                if (triggerType == TriggerType.GreaterThanOrEqual)
                {
                    if (val >= triggerValue)
                    {
                        // 触发报警
                        await AlarmStartAsync(alarmDefinition);
                    }
                    else
                    {
                        await AlarmEndAsync(alarmDefinition);
                    }
                }

                if (triggerType == TriggerType.LessThanOrEqual)
                {
                    if (val >= triggerValue)
                    {
                        // 触发报警
                        await AlarmStartAsync(alarmDefinition);
                    }
                    else
                    {
                        await AlarmEndAsync(alarmDefinition);
                    }
                }
            }
        }
    }

    private async Task AlarmStartAsync(TsjyAlarmDefinition alarmDefinition)
    {
        if (_currentAlarmDict.TryAdd(alarmDefinition.Id, 0))
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var tsjyDbContext = serviceProvider.GetRequiredService<TsjyDbContext>();
            var alarmHistory = new TsjyAlarmHistory
            {
                AlarmDefinitionId = alarmDefinition.Id,
                FacilityId = alarmDefinition.FacilityId,
                StartTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
                Message = alarmDefinition.Message
            };
            await tsjyDbContext.AlarmHistories.AddAsync(alarmHistory);
            await tsjyDbContext.SaveChangesAsync();
            _currentAlarmDict[alarmDefinition.Id] = alarmHistory.Id;
            _logger.LogDebug("alarm start: {message}", alarmDefinition.Message);
        }
    }

    private async Task AlarmEndAsync(TsjyAlarmDefinition alarmDefinition)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var tsjyDbContext = serviceProvider.GetRequiredService<TsjyDbContext>();

        if (_currentAlarmDict.TryGetValue(alarmDefinition.Id, out var alarmHistoryId))
        {
            // 如果应用一直正常运行，应该可以一直获取到对应的报警历史记录 id
            var alarmHistory = await tsjyDbContext.AlarmHistories.FindAsync(alarmHistoryId);
            if (alarmHistory is null)
            {
                // 正常不应为 null
                return;
            }

            alarmHistory.EndTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            var duration = alarmHistory.EndTime - alarmHistory.StartTime;
            // 持续时间至少为 1 秒
            alarmHistory.Duration = duration > 0 ? duration : 1;
            await tsjyDbContext.SaveChangesAsync();
            _currentAlarmDict.Remove(alarmDefinition.Id);
            _logger.LogDebug("alarm end: {message}", alarmDefinition.Message);
        }


        if (await tsjyDbContext.AlarmHistories.AnyAsync(history =>
                history.AlarmDefinitionId == alarmDefinition.Id
                && history.EndTime == null)
           )
        {
            /*
             * 出现这种情况的原因一般是应用采集到了报警开始，并记录了报警历史 id
             * 但是因为如应用重启等原因，_currentAlarmDict 丢失了对应的报警历史 id，导致数据库存在未正常结束的报警记录
             * 这种情况下，直接结束报警定义对应的报警，但是将持续时间标记为异常的 0
             */
            var alarmHistories = tsjyDbContext.AlarmHistories
                .Where(history => history.AlarmDefinitionId == alarmDefinition.Id)
                .Where(history => history.EndTime == null);
            foreach (var alarmHistory in alarmHistories)
            {
                alarmHistory.EndTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                alarmHistory.Duration = 0;
            }

            await tsjyDbContext.SaveChangesAsync();
            _logger.LogWarning("Alarm end with exception: {message}", alarmDefinition.Message);
        }
    }

    private List<TsjyAlarmDefinition> _alarmDefinitions = [];

    public async Task RefreshAlarmDefinitionsAsync()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var tsjyDbContext = serviceProvider.GetRequiredService<TsjyDbContext>();
        _alarmDefinitions = await tsjyDbContext.AlarmDefinitions.ToListAsync();
    }

    public async Task CleanupHistoryAsync()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var tsjyDbContext = scope.ServiceProvider.GetRequiredService<TsjyDbContext>();

        // 清理丢失报警定义 id 并且没有 end time 的报警历史记录
        var unavailableAlarmHistories = tsjyDbContext.AlarmHistories
            .Where(history => history.EndTime == null)
            .Where(history => !tsjyDbContext.AlarmDefinitions
                .Any(definition => definition.Id == history.AlarmDefinitionId));
        tsjyDbContext.AlarmHistories.RemoveRange(unavailableAlarmHistories);

        // 清理超过 30 天的报警历史记录
        var now = DateTimeOffset.Now.ToUnixTimeSeconds();
        var alarmHistories = tsjyDbContext.AlarmHistories
            .Where(history => history.EndTime != null)
            .Where(history => now - history.EndTime > 30 * 24 * 60 * 60);
        tsjyDbContext.AlarmHistories.RemoveRange(alarmHistories);

        await tsjyDbContext.SaveChangesAsync();
    }
}
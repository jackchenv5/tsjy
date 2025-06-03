using Faoem.FacilityStatus.Dtos;
using Faoem.FacilityStatus.Models;
using Faoem.FacilityStatus.Services.StatusBindingService;
using Faoem.Shift.Services.Shift;
using Faoem.Variable.Definitions;
using Faoem.Variable.EventArgs;
using Faoem.Variable.Services.InfluxDbClient;
using Faoem.Variable.Services.Variable;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Faoem.FacilityStatus.Services.StatusRecord;

public class StatusRecordService : IHostedService
{
    private readonly IVariableService _variableService;
    private readonly IInfluxDbClientService _influxDbClientService;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IStatusBindingService _statusBindingService;

    // <设备 id, 设备当前状态>
    private Dictionary<long, Status> _currentFacilityStatus = new();

    public StatusRecordService(
        IVariableService variableService,
        IInfluxDbClientService influxDbClientService,
        IServiceScopeFactory serviceScopeFactory,
        IStatusBindingService statusBindingService
    )
    {
        _variableService = variableService;
        _influxDbClientService = influxDbClientService;
        _serviceScopeFactory = serviceScopeFactory;
        _statusBindingService = statusBindingService;

        _variableService.VariableChangedAsync += VariableServiceOnVariableChangedAsync;
    }

    private async Task VariableServiceOnVariableChangedAsync(VariableChangedEventArgs arg)
    {
        var variables = arg.Variables;
        var bindings = await _statusBindingService.GetStatusBindingsAsync();

        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var shiftService = serviceProvider.GetRequiredService<IShiftService>();
        var currentShift = await shiftService.GetCurrentShiftAsync();

        // 变量变化涉及到的设备 id 列表
        var facilityIds = new List<long>();
        foreach (var variable in variables)
        {
            var binding = bindings.FirstOrDefault(binding =>
                binding.ConnectorInstance == variable.ConnectorInstance &&
                binding.ConnectionName == variable.ConnectionName &&
                binding.DataPoint == variable.DataPointName &&
                binding.Name == variable.Name);
            if (binding is null)
            {
                continue;
            }

            if (variable.DataType != PortalTypes.Bool)
            {
                // 仅支持布尔类型的变量
                continue;
            }

            if (!facilityIds.Contains(binding.FacilityId))
            {
                facilityIds.Add(binding.FacilityId);
            }
        }

        foreach (var facilityId in facilityIds)
        {
            var currentFacilityBindings = bindings
                .Where(binding => binding.FacilityId == facilityId)
                .ToList();
            // 通过所有变量查找当前设备绑定的所有状态变量，状态具有优先级，通过所有状态判断当前设备的实际状态
            var allVariables = await _variableService.GetVariablesAsync();

            bool isRunning;
            var runningBinding = currentFacilityBindings.FirstOrDefault(statusBinding =>
                statusBinding.BindingType == BindingType.Running);
            if (runningBinding is null)
            {
                isRunning = false;
            }
            else
            {
                var runningVariable = allVariables.FirstOrDefault(v =>
                    v.ConnectorInstance == runningBinding.ConnectorInstance &&
                    v.ConnectionName == runningBinding.ConnectionName &&
                    v.DataPointName == runningBinding.DataPoint &&
                    v.Name == runningBinding.Name
                );
                isRunning = (bool)(runningVariable?.Val == true);
            }

            bool isStandby;
            var standbyBinding = currentFacilityBindings.FirstOrDefault(statusBinding =>
                statusBinding.BindingType == BindingType.Standby);
            if (standbyBinding is null)
            {
                isStandby = false;
            }
            else
            {
                var standbyVariable = allVariables.FirstOrDefault(v =>
                    v.ConnectorInstance == standbyBinding.ConnectorInstance &&
                    v.ConnectionName == standbyBinding.ConnectionName &&
                    v.DataPointName == standbyBinding.DataPoint &&
                    v.Name == standbyBinding.Name
                );
                isStandby = (bool)(standbyVariable?.Val == true);
            }

            bool isStopped;
            var stoppedBinding = currentFacilityBindings.FirstOrDefault(statusBinding =>
                statusBinding.BindingType == BindingType.Stopped);
            if (stoppedBinding is null)
            {
                isStopped = false;
            }
            else
            {
                var stoppedVariable = allVariables.FirstOrDefault(v =>
                    v.ConnectorInstance == stoppedBinding.ConnectorInstance &&
                    v.ConnectionName == stoppedBinding.ConnectionName &&
                    v.DataPointName == stoppedBinding.DataPoint &&
                    v.Name == stoppedBinding.Name
                );
                isStopped = (bool)(stoppedVariable?.Val == true);
            }

            bool isError;
            var errorBinding = currentFacilityBindings.FirstOrDefault(statusBinding =>
                statusBinding.BindingType == BindingType.Error);
            if (errorBinding is null)
            {
                isError = true;
            }
            else
            {
                var errorVariable = allVariables.FirstOrDefault(v =>
                    v.ConnectorInstance == errorBinding.ConnectorInstance &&
                    v.ConnectionName == errorBinding.ConnectionName &&
                    v.DataPointName == errorBinding.DataPoint &&
                    v.Name == errorBinding.Name
                );
                isError = (bool)(errorVariable?.Val == true);
            }

            Status newStatus;
            if (isError)
            {
                newStatus = Status.Error;
            }
            else if (isStopped)
            {
                newStatus = Status.Stopped;
            }
            else if (isStandby)
            {
                newStatus = Status.Standby;
            }
            else if (isRunning)
            {
                newStatus = Status.Running;
            }
            else
            {
                newStatus = Status.Invalid;
            }

            if (_currentFacilityStatus.TryGetValue(facilityId, out var status))
            {
                if (newStatus == status)
                {
                    // 如果新状态和当前状态相同，不做任何处理
                    continue;
                }

                // 否则，更新状态
                _currentFacilityStatus[facilityId] = newStatus;
            }
            else
            {
                // 或者添加新状态
                _currentFacilityStatus.TryAdd(facilityId, newStatus);
            }

            // 将新状态写入 InfluxDB
            var tags = new List<(string, string)>
            {
                ("facilityId", $"{facilityId}"),
                ("shiftId", $"{currentShift?.Id}"),
            };
            await _influxDbClientService.WriteDataAsync(tags, ($"{newStatus}", true));
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
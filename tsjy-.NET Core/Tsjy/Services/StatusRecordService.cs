using Faoem.Variable.EventArgs;
using Faoem.Variable.Services.Variable;
using Tsjy.Dtos;
using Tsjy.Models;

namespace Tsjy.Services;

public class StatusRecordService
{
    private readonly IVariableService _variableService;
    private readonly StatusBindingService _statusBindingService;
    private readonly ILogger<StatusRecordService> _logger;

    // <设备 id, 切割机状态>
    private readonly Dictionary<long, SawingMachineStatus> _currentFacilityStatus = new();

    public StatusRecordService(
        IVariableService variableService,
        StatusBindingService statusBindingService,
        ILogger<StatusRecordService> logger)
    {
        _variableService = variableService;
        _statusBindingService = statusBindingService;
        _logger = logger;

        _variableService.VariableChangedAsync += VariableServiceOnVariableChangedAsync;
    }

    private async Task VariableServiceOnVariableChangedAsync(VariableChangedEventArgs arg)
    {
        var variables = arg.Variables;
        var bindings = await _statusBindingService.GetStatusBindingsAsync();

        foreach (var variable in variables)
        {
            var binding = bindings.FirstOrDefault(binding =>
                binding.ConnectorInstance == variable.ConnectorInstance &&
                binding.ConnectionName == variable.ConnectionName &&
                binding.DataPoint == variable.DataPointName &&
                binding.Name == variable.Name);
            if (binding is null)
            {
                // 不是切割机特定状态绑定的的变量，绑定为 null
                continue;
            }

            var facilityId = binding.FacilityId;
            if (!_currentFacilityStatus.TryGetValue(facilityId, out _))
            {
                _currentFacilityStatus[facilityId] = new SawingMachineStatus();
            }

            _logger.LogDebug("FacilityId: {FacilityId}, BindingType: {BindingType}, Val: {Val}", facilityId,
                binding.BindingType, (string?)variable.Val?.ToString());
            switch (binding.BindingType)
            {
                case StatusBindingType.AutoStepNum:
                    if (int.TryParse(variable.Val?.ToString(), out int autoStepNum))
                    {
                        _currentFacilityStatus[facilityId].AutoStepNum = autoStepNum;
                    }

                    break;
                case StatusBindingType.LineVelocity:
                    if (double.TryParse(variable.Val?.ToString(), out double lineVelocity))
                    {
                        _currentFacilityStatus[facilityId].LineVelocity = lineVelocity;
                    }

                    break;
                case StatusBindingType.NewLineLength:
                    if (double.TryParse(variable.Val?.ToString(), out double newLineLength))
                    {
                        _currentFacilityStatus[facilityId].NewLineLength = newLineLength;
                    }

                    break;
                case StatusBindingType.CurrentFeedSpeed:
                    if (double.TryParse(variable.Val?.ToString(), out double currentFeedSpeed))
                    {
                        _currentFacilityStatus[facilityId].CurrentFeedSpeed = currentFeedSpeed;
                    }

                    break;
                case StatusBindingType.TotalCutHeight:
                    if (double.TryParse(variable.Val?.ToString(), out double totalCutHeight))
                    {
                        _currentFacilityStatus[facilityId].TotalCutHeight = totalCutHeight;
                    }

                    break;
                case StatusBindingType.CutHeight:
                    if (double.TryParse(variable.Val?.ToString(), out double cutHeight))
                    {
                        _currentFacilityStatus[facilityId].CutHeight = cutHeight;
                    }

                    break;
                case StatusBindingType.TensionValueTop:
                    if (double.TryParse(variable.Val?.ToString(), out double tensionValueTop))
                    {
                        _currentFacilityStatus[facilityId].TensionValueTop = tensionValueTop;
                    }

                    break;
                case StatusBindingType.TensionValueBottom:
                    if (double.TryParse(variable.Val?.ToString(), out double tensionValueBottom))
                    {
                        _currentFacilityStatus[facilityId].TensionValueBottom = tensionValueBottom;
                    }

                    break;
                case StatusBindingType.MainWheelDiameter:
                    if (double.TryParse(variable.Val?.ToString(), out double mainWheelDiameter))
                    {
                        _currentFacilityStatus[facilityId].MainWheelDiameter = mainWheelDiameter;
                    }

                    break;
                case StatusBindingType.CustomerCode:
                    _currentFacilityStatus[facilityId].CustomerCode = variable.Val?.ToString();
                    break;
                case StatusBindingType.MaterialSpecification:
                    _currentFacilityStatus[facilityId].MaterialSpecification = variable.Val?.ToString();
                    break;
                case StatusBindingType.CutPercentage:
                    if (int.TryParse(variable.Val?.ToString(), out int cutPercentage))
                    {
                        _currentFacilityStatus[facilityId].CutPercentage = cutPercentage;
                    }

                    break;
                case StatusBindingType.TotalCutTime:
                    _currentFacilityStatus[facilityId].TotalCutTime = variable.Val?.ToString();
                    break;
                case StatusBindingType.RemainingTime:
                    _currentFacilityStatus[facilityId].RemainingTime = variable.Val?.ToString();
                    break;
                default:
                    // 处理下一个变量
                    continue;
            }
        }
    }

    public async Task<SawingMachineStatus> GetSawingMachineStatusAsync(long facilityId)
    {
        if (_currentFacilityStatus.TryGetValue(facilityId, out var value))
        {
            return await Task.FromResult(value);
        }

        value = new SawingMachineStatus();
        _currentFacilityStatus[facilityId] = value;

        return await Task.FromResult(value);
    }
}
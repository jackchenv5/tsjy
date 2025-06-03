using Faoem.Variable.EventArgs;
using Faoem.Variable.Services.InfluxDbClient;
using Faoem.Variable.Services.Variable;
using System.Linq;
using Tsjy.Definitions;
using Tsjy.Dtos;
using Tsjy.Eunms;

namespace Tsjy.Services;

public class MotorRecordService
{
    // singleton
    // 使用单例服务保存电机的实时状态，为定时归档提供数据

    private readonly IVariableService _variableService;
    private readonly IInfluxDbClientService _influxDbClientService;
    private readonly MotorBindingService _motorBindingService;

    private readonly Dictionary<long, MotorData> _motorDataDict = [];
    
    public MotorRecordService(
        IVariableService variableService,
        IInfluxDbClientService influxDbClientService,
        MotorBindingService motorBindingService
    )
    {
        _variableService = variableService;
        _influxDbClientService = influxDbClientService;
        _motorBindingService = motorBindingService;

        _variableService.VariableChangedAsync += VariableServiceOnVariableChangedAsync;
    }

    private async Task VariableServiceOnVariableChangedAsync(VariableChangedEventArgs arg)
    {
        var motorBindings = await _motorBindingService.GetMotorBindingsAsync();
        var variables = arg.Variables;
        // 检查是否是电机绑定的变量
        foreach (var variable in variables)
        {
            var motorBinding = motorBindings.FirstOrDefault(binding =>
                binding.ConnectorInstance == variable.ConnectorInstance &&
                binding.ConnectionName == variable.ConnectionName &&
                binding.DataPoint == variable.DataPointName &&
                binding.Name == variable.Name);
            if (motorBinding is null)
            {
                // 不是电机绑定的变量
                continue;
            }

            // 获取指定电机的数据 
            if (!_motorDataDict.TryGetValue(motorBinding.MotorId, out var motorData))
            {
                motorData = new MotorDataDto();
                _motorDataDict.Add(motorBinding.MotorId, motorData);
            }

            // 更新电机数据
            switch (motorBinding.BindingType)
            {
                case MotorBinding.Vibration:
                    if (double.TryParse(variable.Val?.ToString(), out double vibration))
                    {
                        motorData.Vibration = vibration;
                    }

                    break;
                case MotorBinding.Tension:
                    if (double.TryParse(variable.Val?.ToString(), out double tension))
                    {
                        motorData.Tension = tension;
                    }

                    break;
                case MotorBinding.FollowError:
                    if (double.TryParse(variable.Val?.ToString(), out double followError))
                    {
                        motorData.FollowError = followError;
                    }

                    break;
                case MotorBinding.Temperature:
                    if (double.TryParse(variable.Val?.ToString(), out double temperature))
                    {
                        motorData.Temperature = temperature;
                    }

                    break;

                case MotorBinding.Current:
                    if (double.TryParse(variable.Val?.ToString(), out double current))
                    {
                        motorData.Current = current;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public async Task ArchiveMotorDataAsync()
    {
        List<(List<(string name, string value)>, (string name, double value))> points = [];
        foreach (var data in _motorDataDict)
        {
            var tags = new List<(string name, string value)>
            {
                ("motorId", $"{data.Key}"),
            };
            var vibrationField = (name: nameof(data.Value.Vibration), value: data.Value.Vibration);
            var tensionField = (name: nameof(data.Value.Tension), value: data.Value.Tension);
            var followErrorField = (name: nameof(data.Value.FollowError), value: data.Value.FollowError);
            var temperatureField = (name: nameof(data.Value.Temperature), value: data.Value.Temperature);
            var currentField = (name: nameof(data.Value.Current), value: data.Value.Current);
            points.Add((tags, vibrationField));
            points.Add((tags, tensionField));
            points.Add((tags, followErrorField));
            points.Add((tags, temperatureField));
            points.Add((tags, currentField));
        }

        await _influxDbClientService.WriteDataAsync(points);
    }

    public async Task<MotorDataDto> GetMotorDataAsync(long motorId)
    {
        var motorBindings = await _motorBindingService.GetMotorBindingsAsync();

        Dictionary<long, MotorDataDto> _motorDataDtoDict = [];

        if (!_motorDataDict.TryGetValue(motorId, out var motorData))
        {
            motorData = new MotorData();
            _motorDataDict.Add(motorId, motorData);
        }

        foreach(var _motorData in _motorDataDict)
        {
            var list = motorBindings.Where(m => m.MotorId == _motorData.Key).ToList();
            double VibrationMax = 0;
            double VibrationMin = 0;
            double TensionMax = 0;
            double TensionMin = 0;
            double FollowErrorMax = 0;
            double FollowErrorMin = 0;
            double TemperatureMax = 0;
            double TemperatureMin = 0;
            double CurrentMax = 0;
            double CurrentMin = 0;

            double VibrationMaxWarning = 0;
            double VibrationMinWarning = 0;
            double TensionMaxWarning = 0;
            double TensionMinWarning = 0;
            double FollowErrorMaxWarning = 0;
            double FollowErrorMinWarning = 0;
            double TemperatureMaxWarning = 0;
            double TemperatureMinWarning = 0;
            double CurrentMaxWarning = 0;
            double CurrentMinWarning = 0;

            double VibrationMaxAlarm = 0;
            double VibrationMinAlarm = 0;
            double TensionMaxAlarm = 0;
            double TensionMinAlarm = 0;
            double FollowErrorMaxAlarm = 0;
            double FollowErrorMinAlarm = 0;
            double TemperatureMaxAlarm = 0;
            double TemperatureMinAlarm = 0;
            double CurrentMaxAlarm = 0;
            double CurrentMinAlarm = 0;

            foreach (var item in list)
            {
                switch (item.BindingType)
                {
                    case (MotorBinding.Vibration):
                        VibrationMax = item.Max;
                        VibrationMin = item.Min;
                        VibrationMaxWarning = item.MaxWarning;
                        VibrationMinWarning = item.MinWarning;
                        VibrationMaxAlarm = item.MaxAlarm;
                        VibrationMinAlarm = item.MinAlarm;
                        break;
                    case (MotorBinding.Tension):
                        TensionMax = item.Max;
                        TensionMin = item.Min;
                        TensionMaxWarning = item.MaxWarning;
                        TensionMinWarning = item.MinWarning;
                        TensionMaxAlarm = item.MaxAlarm;
                        TensionMinAlarm = item.MinAlarm;
                        break;
                    case (MotorBinding.FollowError):
                        FollowErrorMax = item.Max;
                        FollowErrorMin = item.Min;
                        FollowErrorMaxWarning = item.MaxWarning;
                        FollowErrorMinWarning = item.MinWarning;
                        FollowErrorMaxAlarm = item.MaxAlarm;
                        FollowErrorMinAlarm = item.MinAlarm;
                        break;
                    case (MotorBinding.Temperature):
                        TemperatureMax = item.Max;
                        TemperatureMin = item.Min;
                        TemperatureMaxWarning = item.MaxWarning;
                        TemperatureMinWarning = item.MinWarning;
                        TemperatureMaxAlarm = item.MaxAlarm;
                        TemperatureMinAlarm = item.MinAlarm;
                        break;
                    case (MotorBinding.Current):
                        CurrentMax = item.Max;
                        CurrentMin = item.Min;
                        CurrentMaxWarning = item.MaxWarning;
                        CurrentMinWarning = item.MinWarning;
                        CurrentMaxAlarm = item.MaxAlarm;
                        CurrentMinAlarm = item.MinAlarm;
                        break;
                }
            }


            MotorDataDto m_Dto = new MotorDataDto();
            m_Dto.VibrationMax = VibrationMax;
            m_Dto.VibrationMin = VibrationMin;
            m_Dto.VibrationMaxWarning = VibrationMaxWarning;
            m_Dto.VibrationMinWarning = VibrationMinWarning;
            m_Dto.VibrationMaxAlarm = VibrationMaxAlarm;
            m_Dto.VibrationMinAlarm = VibrationMinAlarm;
            m_Dto.Vibration = _motorData.Value.Vibration;

            m_Dto.TensionMax = TensionMax;
            m_Dto.TensionMin = TensionMin;
            m_Dto.TensionMaxWarning = TensionMaxWarning;
            m_Dto.TensionMinWarning = TensionMinWarning;
            m_Dto.TensionMaxAlarm = TensionMaxAlarm;
            m_Dto.TensionMinAlarm = TensionMinAlarm;
            m_Dto.Tension = _motorData.Value.Tension;

            m_Dto.FollowErrorMax = FollowErrorMax;
            m_Dto.FollowErrorMin = FollowErrorMin;
            m_Dto.FollowErrorMaxWarning = FollowErrorMaxWarning;
            m_Dto.FollowErrorMinWarning = FollowErrorMinWarning;
            m_Dto.FollowErrorMaxAlarm = FollowErrorMaxAlarm;
            m_Dto.FollowErrorMinAlarm = FollowErrorMinAlarm;
            m_Dto.FollowError = _motorData.Value.FollowError;

            m_Dto.TemperatureMax = TemperatureMax;
            m_Dto.TemperatureMin = TemperatureMin;
            m_Dto.TemperatureMaxWarning = TemperatureMaxWarning;
            m_Dto.TemperatureMinWarning = TemperatureMinWarning;
            m_Dto.TemperatureMaxAlarm = TemperatureMaxAlarm;
            m_Dto.TemperatureMinAlarm = TemperatureMinAlarm;
            m_Dto.Temperature = _motorData.Value.Temperature;

            m_Dto.CurrentMax = CurrentMax;
            m_Dto.CurrentMin = CurrentMin;
            m_Dto.CurrentMaxWarning = CurrentMaxWarning;
            m_Dto.CurrentMinWarning = CurrentMinWarning;
            m_Dto.CurrentMaxAlarm = CurrentMaxAlarm;
            m_Dto.CurrentMinAlarm = CurrentMinAlarm;
            m_Dto.Current = _motorData.Value.Current;

            _motorDataDtoDict.Add(_motorData.Key, m_Dto);
        }


        return await Task.FromResult(_motorDataDtoDict[motorId]);
    }

    public async Task<GetMotorHistoryDto> GetMotorDataAsync(long motorId, long startTime, long endTime)
    {
        // var t1 = DateTimeOffset.Now;

        // 将 startTime 和 endTime 转换为 "yyyy-MM-ddTHH:mm:ssZ" 格式
        var startTimeStr = DateTimeOffset.FromUnixTimeSeconds(startTime).ToString("yyyy-MM-ddTHH:mm:ssZ");
        var endTimeStr = DateTimeOffset.FromUnixTimeSeconds(endTime).ToString("yyyy-MM-ddTHH:mm:ssZ");

        var tags = new List<(string, string)>
        {
            ("motorId", $"{motorId}"),
        };

        var fluxTables = await _influxDbClientService.QueryDataAsync(tags, startTimeStr, endTimeStr);

        var fluxRecords = fluxTables
            .SelectMany(table => table.Records)
            .ToList();
        var startTs = fluxRecords.First().GetTime()?.ToUnixTimeSeconds();
        var endTs = fluxRecords.Last().GetTime()?.ToUnixTimeSeconds();
        if (startTs is null || endTs is null)
        {
            return new GetMotorHistoryDto();
        }


        var vibrationData = fluxRecords
            .Where(record => record.GetField() == nameof(MotorDataDto.Vibration))
            .Select(record => new MotorHistoryDataDto
            {
                Time = (long)record.GetTime()?.ToUnixTimeSeconds()!,
                Value = (double)record.GetValue()
            })
            .ToList();

        var tensionData = fluxRecords
            .Where(record => record.GetField() == nameof(MotorDataDto.Tension))
            .Select(record => new MotorHistoryDataDto
            {
                Time = (long)record.GetTime()?.ToUnixTimeSeconds()!,
                Value = (double)record.GetValue()
            })
            .ToList();

        var followErrorData = fluxRecords
            .Where(record => record.GetField() == nameof(MotorDataDto.FollowError))
            .Select(record => new MotorHistoryDataDto
            {
                Time = (long)record.GetTime()?.ToUnixTimeSeconds()!,
                Value = (double)record.GetValue()
            })
            .ToList();

        var temperatureData = fluxRecords
            .Where(record => record.GetField() == nameof(MotorDataDto.Temperature))
            .Select(record => new MotorHistoryDataDto
            {
                Time = (long)record.GetTime()?.ToUnixTimeSeconds()!,
                Value = (double)record.GetValue()
            })
            .ToList();

        var currentData = fluxRecords
            .Where(record => record.GetField() == nameof(MotorDataDto.Current))
            .Select(record => new MotorHistoryDataDto
            {
                Time = (long)record.GetTime()?.ToUnixTimeSeconds()!,
                Value = (double)record.GetValue()
            })
            .ToList();

        var result = new GetMotorHistoryDto
        {
            VibrationData = [],
            TensionData = [],
            FollowErrorData = [],
            TemperatureErrorData = [],
            CurrentErrorData = []
        };
        
        
        for (var i = 0; i <= endTs - startTs; i++)
        {
            var ts = (long)startTs + i;
            if (vibrationData[0].Time == ts)
            {
                result.VibrationData.Add(vibrationData[0]);
                vibrationData.RemoveAt(0);
            }
            else
            {
                result.VibrationData.Add(new MotorHistoryDataDto()
                {
                    Time = ts,
                    Value = null
                });
            }

            if (tensionData[0].Time == ts)
            {
                result.TensionData.Add(tensionData[0]);
                tensionData.RemoveAt(0);
            }
            else
            {
                result.TensionData.Add(new MotorHistoryDataDto()
                {
                    Time = ts,
                    Value = null
                });
            }

            if (followErrorData[0].Time == ts)
            {
                result.FollowErrorData.Add(followErrorData[0]);
                followErrorData.RemoveAt(0);
            }
            else
            {
                result.FollowErrorData.Add(new MotorHistoryDataDto()
                {
                    Time = ts,
                    Value = null
                });
            }

            if (temperatureData[0].Time == ts)
            {
                result.TemperatureErrorData.Add(temperatureData[0]);
                temperatureData.RemoveAt(0);
            }
            else
            {
                result.TemperatureErrorData.Add(new MotorHistoryDataDto()
                {
                    Time = ts,
                    Value = null
                });
            }

            if (currentData[0].Time == ts)
            {
                result.CurrentErrorData.Add(currentData[0]);
                currentData.RemoveAt(0);
            }
            else
            {
                result.CurrentErrorData.Add(new MotorHistoryDataDto()
                {
                    Time = ts,
                    Value = null
                });
            }
        }

        // var t2 = DateTimeOffset.Now;

        // Console.WriteLine($"-------- {(t2 - t1).TotalSeconds:F3} --------");

        return result;
    }
}
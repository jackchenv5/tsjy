using Tsjy.Definitions;

namespace Tsjy;

public class MotorDataDto: MotorData
{
    public double VibrationMax { get; set; }
    public double VibrationMin { get; set; }
    public double TensionMax { get; set; }
    public double TensionMin { get; set; }
    public double FollowErrorMax { get; set; }
    public double FollowErrorMin { get; set; }

    public double TemperatureMax { get; set; }
    public double TemperatureMin { get; set; }

    public double CurrentMax { get; set; }
    public double CurrentMin { get; set; }

    public double VibrationMaxWarning { get; set; }
    public double VibrationMinWarning { get; set; }
    public double TensionMaxWarning { get; set; }
    public double TensionMinWarning { get; set; }
    public double FollowErrorMaxWarning { get; set; }
    public double FollowErrorMinWarning { get; set; }

    public double TemperatureMaxWarning { get; set; }
    public double TemperatureMinWarning { get; set; }

    public double CurrentMaxWarning { get; set; }
    public double CurrentMinWarning { get; set; }

    public double VibrationMaxAlarm { get; set; }
    public double VibrationMinAlarm { get; set; }
    public double TensionMaxAlarm { get; set; }
    public double TensionMinAlarm { get; set; }
    public double FollowErrorMaxAlarm { get; set; }
    public double FollowErrorMinAlarm { get; set; }

    public double TemperatureMaxAlarm { get; set; }
    public double TemperatureMinAlarm { get; set; }

    public double CurrentMaxAlarm { get; set; }
    public double CurrentMinAlarm { get; set; }
}
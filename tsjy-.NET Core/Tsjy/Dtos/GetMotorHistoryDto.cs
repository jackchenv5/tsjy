namespace Tsjy.Dtos;

public class GetMotorHistoryDto
{
    public List<MotorHistoryDataDto> VibrationData { get; set; } = null!;
    public List<MotorHistoryDataDto> TensionData { get; set; } = null!;
    public List<MotorHistoryDataDto> FollowErrorData { get; set; } = null!;
    public List<MotorHistoryDataDto> TemperatureErrorData { get; set; } = null!;
    public List<MotorHistoryDataDto> CurrentErrorData { get; set; } = null!;
}
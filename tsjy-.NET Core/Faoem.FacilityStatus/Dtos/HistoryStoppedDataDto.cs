namespace Faoem.FacilityStatus.Dtos;

/// <summary>
/// 历史停机数据
/// </summary>
public class HistoryStoppedDataDto
{
    /// <summary>
    /// 待机时长列表
    /// </summary>
    public List<int> StandbySecondList { get; set; } = [];

    /// <summary>
    /// 停机时长列表
    /// </summary>
    public List<int> StoppedMinuteList { get; set; } = [];

    /// <summary>
    /// 故障时长列表
    /// </summary>
    public List<int> ErrorMinuteList { get; set; } = [];
}
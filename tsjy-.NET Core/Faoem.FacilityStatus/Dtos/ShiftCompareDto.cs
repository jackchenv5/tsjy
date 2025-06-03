namespace Faoem.FacilityStatus.Dtos;

/// <summary>
/// 班次停机时常对比
/// </summary>
public class ShiftCompareDto
{
    /// <summary>
    /// 班组名列表
    /// </summary>
    public List<string> ShiftNames { get; set; } = [];

    /// <summary>
    /// 待机时长列表
    /// </summary>
    public List<int> StandbySeconds { get; set; } = [];

    /// <summary>
    /// 停机时长列表
    /// </summary>
    public List<int> StoppedSeconds { get; set; } = [];

    /// <summary>
    /// 故障时长列表
    /// </summary>
    public List<int> ErrorSeconds { get; set; } = [];
}
namespace Faoem.FacilityStatus.Inputs;

public class StatusQueryInput
{
    /// <summary>
    /// 设备 id
    /// </summary>
    public long FacilityId { get; set; }

    /// <summary>
    /// 开始时间（含）
    /// </summary>
    public long StartTime { get; set; }

    /// <summary>
    /// 结束时间（不含）
    /// </summary>
    public long EndTime { get; set; }

    /// <summary>
    /// 当前页
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数据量
    /// </summary>
    public int PageSize { get; set; } = 20;
}
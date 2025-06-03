namespace Tsjy.Dtos;

public class SawingMachineStatus
{
    /// <summary>
    /// 段号
    /// </summary>
    public int AutoStepNum { get; set; }

    /// <summary>
    /// 切线速度
    /// </summary>
    public double LineVelocity { get; set; }

    /// <summary>
    /// 新线进给量
    /// </summary>
    public double NewLineLength { get; set; }

    /// <summary>
    /// 进给速度
    /// </summary>
    public double CurrentFeedSpeed { get; set; }

    /// <summary>
    /// 切割总高度
    /// </summary>
    public double TotalCutHeight { get; set; }

    /// <summary>
    /// 已切割高度
    /// </summary>
    public double CutHeight { get; set; }

    /// <summary>
    /// 上摆杆张力设定值
    /// </summary>
    public double TensionValueTop { get; set; }

    /// <summary>
    /// 下摆杆张力设定值
    /// </summary>
    public double TensionValueBottom { get; set; }

    /// <summary>
    /// 槽轮直径
    /// </summary>
    public double MainWheelDiameter { get; set; }

    /// <summary>
    /// 客户代码
    /// </summary>
    public string? CustomerCode { get; set; }

    /// <summary>
    /// 切割材料规格
    /// </summary>
    public string? MaterialSpecification { get; set; }

    /// <summary>
    /// 切割百分比
    /// </summary>
    public int CutPercentage { get; set; }

    /// <summary>
    /// 切割总时间
    /// </summary>
    public string? TotalCutTime { get; set; }

    /// <summary>
    /// 剩余时间
    /// </summary>
    public string? RemainingTime { get; set; }
}
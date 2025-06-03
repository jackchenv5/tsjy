using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tsjy.Models;

public enum StatusBindingType
{
    /// <summary>
    /// 段号
    /// </summary>
    AutoStepNum = 1,

    /// <summary>
    /// 切线速度
    /// </summary>
    LineVelocity = 2,

    /// <summary>
    /// 新线进给量
    /// </summary>
    NewLineLength = 3,

    /// <summary>
    /// 进给速度
    /// </summary>
    CurrentFeedSpeed = 4,

    /// <summary>
    /// 切割总高度
    /// </summary>
    TotalCutHeight = 5,

    /// <summary>
    /// 已切割高度
    /// </summary>
    CutHeight = 6,

    /// <summary>
    /// 上摆杆张力设定值
    /// </summary>
    TensionValueTop = 7,

    /// <summary>
    /// 下摆杆张力设定值
    /// </summary>
    TensionValueBottom = 8,

    /// <summary>
    /// 槽轮直径
    /// </summary>
    MainWheelDiameter = 9,

    /// <summary>
    /// 客户代码
    /// </summary>
    CustomerCode = 10,

    /// <summary>
    /// 切割材料规格
    /// </summary>
    MaterialSpecification = 11,

    /// <summary>
    /// 切割百分比
    /// </summary>
    CutPercentage = 12,

    /// <summary>
    /// 切割总时间
    /// </summary>
    TotalCutTime = 13,

    /// <summary>
    /// 剩余时间
    /// </summary>
    RemainingTime = 14,
}

[EntityTypeConfiguration(typeof(TsjyStatusBinding))]
public class TsjyStatusBinding : IEntityTypeConfiguration<TsjyStatusBinding>
{
    public long Id { get; set; }
    public StatusBindingType BindingType { get; set; }
    [MaxLength(128)] public string ConnectorInstance { get; set; } = null!;
    [MaxLength(128)] public string ConnectionName { get; set; } = null!;
    [MaxLength(128)] public string DataPoint { get; set; } = null!;
    [MaxLength(256)] public string Name { get; set; } = null!;
    public long FacilityId { get; set; }

    public void Configure(EntityTypeBuilder<TsjyStatusBinding> builder)
    {
        builder.ToTable("tsjy_status_binding");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(m => m.BindingType)
            .HasColumnName("binding_type");

        builder.Property(m => m.ConnectorInstance)
            .HasColumnName("connector_instance");

        builder.Property(m => m.ConnectionName)
            .HasColumnName("connection_name");

        builder.Property(m => m.DataPoint)
            .HasColumnName("data_point");

        builder.Property(m => m.Name)
            .HasColumnName("name");

        builder.Property(m => m.FacilityId)
            .HasColumnName("facility_id");
    }
}
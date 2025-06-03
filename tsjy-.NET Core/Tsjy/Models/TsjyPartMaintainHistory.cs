using System.ComponentModel.DataAnnotations;
using Faoem.Facility.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tsjy.Models;

[EntityTypeConfiguration(typeof(TsjyPartMaintainHistory))]
public class TsjyPartMaintainHistory : IEntityTypeConfiguration<TsjyPartMaintainHistory>
{
    public long Id { get; set; }
    [MaxLength(128)] public string PartName { get; set; } = null!;
    [MaxLength(128)] public string? Remark { get; set; }
    public long FacilityId { get; set; }
    public virtual Facility? Facility { get; set; }

    /// <summary>
    /// 处理时间
    /// </summary>
    public long ProcessTime { get; set; }

    /// <summary>
    /// 数据记录的时间
    /// </summary>
    public long Time { get; set; }

    [MaxLength(256)] public string? Reason { get; set; }
    [MaxLength(256)] public string? ProcessMethod { get; set; }

    public void Configure(EntityTypeBuilder<TsjyPartMaintainHistory> builder)
    {
        builder.ToTable("tsjy_part_maintain_history");

        builder.HasKey(history => history.Id);
        builder.Property(history => history.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(history => history.PartName)
            .HasColumnName("part_name");

        builder.Property(history => history.Remark)
            .HasColumnName("remark");

        builder.Property(history => history.FacilityId)
            .HasColumnName("facility_id");

        builder.Ignore(history => history.Facility);

        builder.Property(history => history.ProcessTime)
            .HasColumnName("process_time");

        builder.Property(history => history.Time)
            .HasColumnName("time");

        builder.Property(history => history.Reason)
            .HasColumnName("reason");

        builder.Property(history => history.ProcessMethod)
            .HasColumnName("process_method");
    }
}
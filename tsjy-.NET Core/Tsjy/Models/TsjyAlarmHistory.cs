using System.ComponentModel.DataAnnotations;
using Faoem.Facility.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tsjy.Models;

[EntityTypeConfiguration(typeof(TsjyAlarmHistory))]
public class TsjyAlarmHistory : IEntityTypeConfiguration<TsjyAlarmHistory>
{
    public long Id { get; set; }

    // 用于统计报警次数，避免使用报警消息进行统计。报警消息是不固定字符串，查询较慢，并且可能修改。
    public long AlarmDefinitionId { get; set; }

    // 用于获取设备信息和统计每个设备的报警
    public long FacilityId { get; set; }
    public virtual Facility? Facility { get; set; }
    public long StartTime { get; set; }
    public long? EndTime { get; set; }
    public long? Duration { get; set; }
    [MaxLength(1024)] public string Message { get; set; } = null!;


    public void Configure(EntityTypeBuilder<TsjyAlarmHistory> builder)
    {
        builder.ToTable("tsjy_alarm_history");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(m => m.AlarmDefinitionId)
            .HasColumnName("alarm_definition_id");

        builder.Property(m => m.FacilityId)
            .HasColumnName("facility_id");

        builder.Ignore(m => m.Facility);

        builder.Property(m => m.StartTime)
            .HasColumnName("start_time");

        builder.Property(m => m.EndTime)
            .HasColumnName("end_time");

        builder.Property(m => m.Duration)
            .HasColumnName("duration");

        builder.Property(m => m.Message)
            .HasColumnName("message");
    }
}
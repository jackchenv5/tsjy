using System.ComponentModel.DataAnnotations;
using Faoem.Facility.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tsjy.Models;

public enum TriggerType
{
    GreaterThan = 0,
    LessThan = 1,
    Equal = 2,
    NotEqual = 3,
    GreaterThanOrEqual = 4,
    LessThanOrEqual = 5
}

[EntityTypeConfiguration(typeof(TsjyAlarmDefinition))]
public class TsjyAlarmDefinition : IEntityTypeConfiguration<TsjyAlarmDefinition>
{
    public long Id { get; set; }
    [MaxLength(128)] public string ConnectorInstance { get; set; } = null!;
    [MaxLength(128)] public string ConnectionName { get; set; } = null!;
    [MaxLength(128)] public string DataPoint { get; set; } = null!;
    [MaxLength(256)] public string Name { get; set; } = null!;
    public long FacilityId { get; set; }
    [MaxLength(1024)] public string Message { get; set; } = null!;
    public TriggerType TriggerType { get; set; }
    [MaxLength(64)] public string DataType { get; set; } = null!;
    [MaxLength(64)] public string TriggerValue { get; set; } = null!;
    public virtual Facility? Facility { get; set; }

    public void Configure(EntityTypeBuilder<TsjyAlarmDefinition> builder)
    {
        builder.ToTable("tsjy_alarm_definition");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

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

        builder.Property(m => m.Message)
            .HasColumnName("message");

        builder.Property(m => m.TriggerType)
            .HasColumnName("trigger_type");

        builder.Property(m => m.DataType)
            .HasColumnName("data_type");

        builder.Property(m => m.TriggerValue)
            .HasColumnName("trigger_value");

        builder.Ignore(m => m.Facility);
    }
}
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tsjy.Eunms;

namespace Tsjy.Models;

[EntityTypeConfiguration(typeof(TsjyMotorBinding))]
public class TsjyMotorBinding : IEntityTypeConfiguration<TsjyMotorBinding>
{
    public long Id { get; set; }
    public long MotorId { get; set; }
    public MotorBinding BindingType { get; set; }
    [MaxLength(128)] public string ConnectorInstance { get; set; } = null!;
    [MaxLength(128)] public string ConnectionName { get; set; } = null!;
    [MaxLength(128)] public string DataPoint { get; set; } = null!;
    [MaxLength(256)] public string Name { get; set; } = null!;

    //threshold
    public double Max { get; set; }
    public double Min { get; set; }

    public double MaxWarning { get; set; }
    public double MinWarning { get; set; }

    public double MaxAlarm { get; set; }
    public double MinAlarm { get; set; }

    public void Configure(EntityTypeBuilder<TsjyMotorBinding> builder)
    {
        builder.ToTable("tsjy_motor_binding");

        builder.HasKey(binding => binding.Id);

        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(binding => binding.MotorId)
            .HasColumnName("motor_id");

        builder.Property(binding => binding.BindingType)
            .HasColumnName("binding_type");

        builder.Property(m => m.ConnectorInstance)
            .HasColumnName("connector_instance");

        builder.Property(m => m.ConnectionName)
            .HasColumnName("connection_name");

        builder.Property(m => m.DataPoint)
            .HasColumnName("data_point");

        builder.Property(m => m.Name)
            .HasColumnName("name");

        builder.Property(m => m.Max)
           .HasColumnName("max");

        builder.Property(m => m.Min)
           .HasColumnName("min");

        builder.Property(m => m.MaxWarning)
           .HasColumnName("max_warning");

        builder.Property(m => m.MinWarning)
           .HasColumnName("min_warning");

        builder.Property(m => m.MaxAlarm)
           .HasColumnName("max_alarm");

        builder.Property(m => m.MinAlarm)
           .HasColumnName("min_alarm");
    }
}
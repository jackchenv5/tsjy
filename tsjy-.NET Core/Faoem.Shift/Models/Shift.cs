using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faoem.Shift.Models;

[EntityTypeConfiguration(typeof(Shift))]
public class Shift : IEntityTypeConfiguration<Shift>
{
    public long Id { get; set; }
    public byte Number { get; set; }
    public bool IsEnabled { get; set; }
    [MaxLength(32)] public string Name { get; set; } = null!;
    public long StartTime { get; set; }
    public long EndTime { get; set; }
    public bool SpanTheDay { get; set; }

    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.ToTable("shift_shift");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id")
            .HasComment("班次 Id");

        builder.Property(m => m.Number)
            .HasColumnName("number")
            .HasComment("班次编号");

        builder.Property(m => m.IsEnabled)
            .HasColumnName("is_enabled")
            .HasComment("是否启用");

        builder.Property(m => m.Name)
            .HasColumnName("name")
            .HasComment("班次名称");

        builder.Property(m => m.StartTime)
            .HasColumnName("start_time")
            .HasComment("开始时间，unix 时间戳");

        builder.Property(m => m.EndTime)
            .HasColumnName("end_time")
            .HasComment("结束时间，unix 时间戳");

        builder.Property(m => m.SpanTheDay)
            .HasColumnName("span_the_day")
            .HasComment("是否跨天");
    }
}
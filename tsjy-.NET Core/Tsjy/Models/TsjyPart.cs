using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tsjy.Models;

[EntityTypeConfiguration(typeof(TsjyPart))]
public class TsjyPart : IEntityTypeConfiguration<TsjyPart>
{
    public long Id { get; set; }
    [MaxLength(128)] public string PartName { get; set; } = null!;
    public long FacilityId { get; set; }
    [MaxLength(128)] public string? Remark { get; set; }

    /// <summary>
    /// 上次的值
    /// </summary>
    public double? LastValue { get; set; }

    /// <summary>
    /// 指寿命数据的更新时间
    /// </summary>
    public long? UpdatedAt { get; set; }

    [MaxLength(128)] public string ConnectorInstance { get; set; } = string.Empty;
    [MaxLength(128)] public string ConnectionName { get; set; } = string.Empty;
    [MaxLength(128)] public string DataPoint { get; set; } = string.Empty;
    [MaxLength(256)] public string VariableName { get; set; } = string.Empty;

    public void Configure(EntityTypeBuilder<TsjyPart> builder)
    {
        builder.ToTable("tsjy_parts");

        builder.HasKey(part => part.Id);
        builder.Property(part => part.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(part => part.PartName)
            .HasColumnName("part_name");

        builder.Property(part => part.FacilityId)
            .HasColumnName("facility_id");

        builder.Property(part => part.Remark)
            .HasColumnName("remark");

        builder.Property(part => part.LastValue)
            .HasColumnName("last_value");

        builder.Property(part => part.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(part => part.ConnectorInstance)
            .HasColumnName("connector_instance");

        builder.Property(part => part.ConnectionName)
            .HasColumnName("connection_name");

        builder.Property(part => part.DataPoint)
            .HasColumnName("data_point");

        builder.Property(part => part.VariableName)
            .HasColumnName("variable_name");
    }
}
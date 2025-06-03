using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faoem.Common.Models;

[EntityTypeConfiguration(typeof(Setting))]
public class Setting : IEntityTypeConfiguration<Setting>
{
    public long Id { get; set; }
    [MaxLength(256)] public string Key { get; set; } = null!;
    [MaxLength(256)] public string? Value { get; set; }

    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.ToTable("common_setting");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(m => m.Key)
            .HasColumnName("key");

        builder.Property(m => m.Value)
            .HasColumnName("value");
    }
}
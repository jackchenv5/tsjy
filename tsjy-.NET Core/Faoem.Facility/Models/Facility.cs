using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faoem.Facility.Models;

[EntityTypeConfiguration(typeof(Facility))]
public class Facility : IEntityTypeConfiguration<Facility>
{
    private string _name = null!;
    private string? _serialNumber;

    public long Id { get; set; }

    [MaxLength(64)]
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            LowerName = value.ToLower();
        }
    }

    [MaxLength(64)] public string LowerName { get; set; } = null!;
    public bool IsEnabled { get; set; }

    [MaxLength(128)]
    public string? SerialNumber
    {
        get => _serialNumber;
        set
        {
            _serialNumber = value;
            LowerSerialNumber = value?.ToLower();
        }
    }

    [MaxLength(128)] public string? LowerSerialNumber { get; set; }
    [MaxLength(256)] public string? Description { get; set; }

    public void Configure(EntityTypeBuilder<Facility> builder)
    {
        builder.ToTable("facility_facility");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(m => m.Name)
            .HasColumnName("name");

        builder.Property(m => m.LowerName)
            .HasColumnName("lower_name");

        builder.Property(m => m.IsEnabled)
            .HasColumnName("is_enabled");

        builder.Property(m => m.SerialNumber)
            .HasColumnName("serial_number");

        builder.Property(m => m.LowerSerialNumber)
            .HasColumnName("lower_serial_number");

        builder.Property(m => m.Description)
            .HasColumnName("description");
    }
}
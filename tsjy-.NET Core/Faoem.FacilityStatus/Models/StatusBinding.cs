using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faoem.FacilityStatus.Models;

public enum BindingType
{
    Invalid = 0,
    Running = 1,
    Standby = 2,
    Stopped = 3,
    Error = 4
}

[EntityTypeConfiguration(typeof(StatusBinding))]
public class StatusBinding : IEntityTypeConfiguration<StatusBinding>
{
    public long Id { get; set; }
    public BindingType BindingType { get; set; }
    [MaxLength(128)] public string ConnectorInstance { get; set; } = null!;
    [MaxLength(128)] public string ConnectionName { get; set; } = null!;
    [MaxLength(128)] public string DataPoint { get; set; } = null!;
    [MaxLength(256)] public string Name { get; set; } = null!;
    public long FacilityId { get; set; }

    public void Configure(EntityTypeBuilder<StatusBinding> builder)
    {
        builder.ToTable("facility_status_facility_status_binding");

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
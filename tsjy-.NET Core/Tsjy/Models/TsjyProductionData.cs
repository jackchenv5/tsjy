using System.ComponentModel.DataAnnotations;
using Faoem.Facility.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tsjy.Models;

[EntityTypeConfiguration(typeof(TsjyProductionData))]
public class TsjyProductionData : IEntityTypeConfiguration<TsjyProductionData>
{
    public long Id { get; set; }
    [MaxLength(256)] public string CustomerCode { get; set; } = string.Empty;
    [MaxLength(256)] public string MaterialSpecification { get; set; } = string.Empty;
    public long CompleteTime { get; set; }
    public long FacilityId { get; set; }
    public virtual Facility? Facility { get; set; }

    public void Configure(EntityTypeBuilder<TsjyProductionData> builder)
    {
        builder.ToTable("tsjy_production_data");

        builder.HasKey(data => data.Id);
        builder.Property(data => data.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(data => data.CustomerCode)
            .HasColumnName("customer_code");

        builder.Property(data => data.MaterialSpecification)
            .HasColumnName("material_specification");

        builder.Property(data => data.CompleteTime)
            .HasColumnName("complete_time");

        builder.Property(data => data.FacilityId)
            .HasColumnName("facility_id");

        builder.Ignore(data => data.Facility);
    }
}
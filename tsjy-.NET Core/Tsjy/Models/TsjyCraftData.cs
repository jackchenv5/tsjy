using System.ComponentModel.DataAnnotations;
using Faoem.Facility.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tsjy.Eunms;

namespace Tsjy.Models;

[EntityTypeConfiguration(typeof(TsjyCraftData))]
public class TsjyCraftData : IEntityTypeConfiguration<TsjyCraftData>
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public virtual Facility? Facility { get; set; }
    public CraftBinding BindingType { get; set; }
    public int Index { get; set; }
    public double Value { get; set; }
    public long Time { get; set; }
    [MaxLength(256)] public string CustomerCode { get; set; } = string.Empty;
    [MaxLength(256)] public string MaterialSpecification { get; set; } = string.Empty;

    public void Configure(EntityTypeBuilder<TsjyCraftData> builder)
    {
        builder.ToTable("tsjy_craft_data");

        builder.HasKey(data => data.Id);
        builder.Property(data => data.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(data => data.FacilityId)
            .HasColumnName("facility_id");

        builder.Ignore(data => data.Facility);

        builder.Property(data => data.BindingType)
            .HasColumnName("binding_type");

        builder.Property(data => data.Index)
            .HasColumnName("index");

        builder.Property(data => data.Value)
            .HasColumnName("value");

        builder.Property(data => data.Time)
            .HasColumnName("time");

        builder.Property(data => data.CustomerCode)
            .HasColumnName("customer_code");

        builder.Property(data => data.MaterialSpecification)
            .HasColumnName("material_specification");
    }
}
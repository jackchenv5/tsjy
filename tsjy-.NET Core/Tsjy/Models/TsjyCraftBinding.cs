using System.ComponentModel.DataAnnotations;
using Faoem.Facility.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tsjy.Eunms;

namespace Tsjy.Models;

[EntityTypeConfiguration(typeof(TsjyCraftBinding))]
public class TsjyCraftBinding : IEntityTypeConfiguration<TsjyCraftBinding>
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public virtual Facility? Facility { get; set; }
    public int Index { get; set; }
    public CraftBinding BindingType { get; set; }
    [MaxLength(128)] public string ConnectorInstance { get; set; } = null!;
    [MaxLength(128)] public string ConnectionName { get; set; } = null!;
    [MaxLength(128)] public string DataPoint { get; set; } = null!;
    [MaxLength(256)] public string Name { get; set; } = null!;

    public void Configure(EntityTypeBuilder<TsjyCraftBinding> builder)
    {
        builder.ToTable("tsjy_craft_binding");

        builder.HasKey(binding => binding.Id);
        builder.Property(binding => binding.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(binding => binding.FacilityId)
            .HasColumnName("facility_id");

        builder.Ignore(binding => binding.Facility);

        builder.Property(binding => binding.Index)
            .HasColumnName("index");

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
    }
}
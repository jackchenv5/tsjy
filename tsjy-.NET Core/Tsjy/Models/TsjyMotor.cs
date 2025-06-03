using System.ComponentModel.DataAnnotations;
using Faoem.Facility.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tsjy.Models;

[EntityTypeConfiguration(typeof(TsjyMotor))]
public class TsjyMotor : IEntityTypeConfiguration<TsjyMotor>
{
    public long Id { get; set; }


    private string _name = null!;

    [MaxLength(256)]
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            LowerName = value.ToLower();
        }
    }

    [MaxLength(256)] public string LowerName { get; set; } = null!;
    public long FacilityId { get; set; }
    public virtual Facility? Facility { get; set; }

    public void Configure(EntityTypeBuilder<TsjyMotor> builder)
    {
        builder.ToTable("tsjy_motor");

        builder.HasKey(motor => motor.Id);
        builder.Property(motor => motor.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(motor => motor.Name)
            .HasColumnName("name");

        builder.Property(motor => motor.FacilityId)
            .HasColumnName("facility_id");

        builder.Ignore(motor => motor.Facility);

        builder.Property(motor => motor.LowerName)
            .HasColumnName("lower_name");
    }
}
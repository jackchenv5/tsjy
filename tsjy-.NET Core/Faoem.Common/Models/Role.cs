using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faoem.Common.Models;

[EntityTypeConfiguration(typeof(Role))]
public class Role : IEntityTypeConfiguration<Role>
{
    private string _roleName = null!;

    public long Id { get; set; }

    [MaxLength(32)]
    public string RoleName
    {
        get => _roleName;
        set
        {
            _roleName = value;
            LowerRoleName = value.ToLower();
        }
    }

    [MaxLength(32)] public string LowerRoleName { get; private set; } = null!;

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("common_role");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(m => m.RoleName)
            .HasColumnName("role_name");

        builder.Property(m => m.LowerRoleName)
            .HasColumnName("lower_role_name");
    }
}
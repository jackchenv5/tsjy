using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faoem.Common.Models;

[EntityTypeConfiguration(typeof(RolePermission))]
public class RolePermission : IEntityTypeConfiguration<RolePermission>
{
    public long RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public virtual Role Role { get; set; } = null!;
    public virtual Permission Permission { get; set; } = null!;

    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("common_role_permission");

        builder.HasKey(m => new { m.RoleId, m.PermissionId });

        builder.Property(m => m.RoleId)
            .HasColumnName("role_id");

        builder.Property(m => m.PermissionId)
            .HasColumnName("permission_id");

        builder.HasOne(m => m.Role)
            .WithMany()
            .HasForeignKey(m => m.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Permission)
            .WithMany()
            .HasForeignKey(m => m.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
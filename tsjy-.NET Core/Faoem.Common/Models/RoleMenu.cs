using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faoem.Common.Models;

[EntityTypeConfiguration(typeof(RoleMenu))]
public class RoleMenu : IEntityTypeConfiguration<RoleMenu>
{
    public long RoleId { get; set; }
    public long MenuId { get; set; }
    public virtual Role Role { get; set; } = null!;
    public virtual Menu Menu { get; set; } = null!;

    public void Configure(EntityTypeBuilder<RoleMenu> builder)
    {
        builder.ToTable("common_role_menu");

        builder.HasKey(m => new { m.RoleId, m.MenuId });

        builder.Property(m => m.RoleId)
            .HasColumnName("role_id");

        builder.Property(m => m.MenuId)
            .HasColumnName("menu_id");

        builder.HasOne(m => m.Menu)
            .WithMany()
            .HasForeignKey(m => m.MenuId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Role)
            .WithMany()
            .HasForeignKey(m => m.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
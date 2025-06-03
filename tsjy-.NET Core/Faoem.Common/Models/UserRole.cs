using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faoem.Common.Models;

[EntityTypeConfiguration(typeof(UserRole))]
public class UserRole : IEntityTypeConfiguration<UserRole>
{
    public long UserId { get; set; }

    public long RoleId { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("common_user_role");

        builder.HasKey(m => new { m.UserId, m.RoleId });

        builder.Property(m => m.UserId)
            .HasColumnName("user_id");

        builder.HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(m => m.RoleId)
            .HasColumnName("role_id");

        builder.HasOne(m => m.Role)
            .WithMany()
            .HasForeignKey(m => m.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
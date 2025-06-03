using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faoem.Common.Models;

[EntityTypeConfiguration(typeof(Permission))]
public class Permission : IEntityTypeConfiguration<Permission>
{
    public Guid Id { get; set; }
    [MaxLength(256)] public string Route { get; set; } = null!;
    [MaxLength(16)] public string HttpMethod { get; set; } = null!;
    [MaxLength(128)] public string ControllerName { get; set; } = null!;
    [MaxLength(256)] public string? ActionDescription { get; set; }

    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("common_permission");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(m => m.Route)
            .HasColumnName("route");

        builder.Property(m => m.HttpMethod)
            .HasColumnName("http_method");

        builder.Property(m => m.ControllerName)
            .HasColumnName("controller_name");

        builder.Property(m => m.ActionDescription)
            .HasColumnName("action_description");
    }
}
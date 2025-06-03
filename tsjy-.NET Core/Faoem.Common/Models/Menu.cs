using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faoem.Common.Models;

[EntityTypeConfiguration(typeof(Menu))]
public class Menu : IEntityTypeConfiguration<Menu>
{
    private string _label = null!;
    private string? _route;

    public long Id { get; set; }

    [MaxLength(32)]
    public string Label
    {
        get => _label;
        set
        {
            _label = value;
            LowerLabel = value.ToLower();
        }
    }

    [MaxLength(32)] public string LowerLabel { get; private set; } = null!;
    public bool IsSubMenu { get; set; }
    public int Order { get; set; }
    public long? ParentId { get; set; }

    [MaxLength(256)]
    public string? Route
    {
        get => _route;
        set
        {
            _route = value;
            LowerRoute = value?.ToLower();
        }
    }

    [MaxLength(256)] public string? LowerRoute { get; private set; }
    public virtual Menu? ParentMenu { get; set; }
    public virtual List<Menu>? Children { get; set; }

    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("common_menu");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(m => m.Label)
            .HasColumnName("label");

        builder.Property(m => m.LowerLabel)
            .HasColumnName("lower_label");

        builder.Property(m => m.IsSubMenu)
            .HasColumnName("is_sub_menu");

        builder.Property(m => m.Order)
            .HasColumnName("order");

        builder.Property(m => m.ParentId)
            .HasColumnName("parent_id");

        builder.HasOne(m => m.ParentMenu)
            .WithMany(m => m.Children)
            .HasForeignKey(m => m.ParentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(m => m.Route)
            .HasColumnName("route");

        builder.Property(m => m.LowerRoute)
            .HasColumnName("lower_route");
    }
}
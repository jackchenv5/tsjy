using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faoem.Common.Models;

[EntityTypeConfiguration(typeof(User))]
public class User : IEntityTypeConfiguration<User>
{
    public long Id { get; set; }

    private string _username = null!;

    [MaxLength(32)]
    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            LowerUsername = value.ToLower();
        }
    }

    [MaxLength(32)] public string LowerUsername { get; private set; } = null!;
    [MaxLength(32)] public string? FullName { get; set; }
    public long CreatedAt { get; set; }
    public long LastLogin { get; set; }
    [MaxLength(64)] public string PasswordHash { get; set; } = null!;
    [MaxLength(32)] public string Salt { get; set; } = null!;

    private string? _email;

    [MaxLength(256)]
    public string? Email
    {
        get => _email;
        set
        {
            _email = value;
            LowerEmail = value?.ToLower();
        }
    }

    [MaxLength(256)] public string? LowerEmail { get; private set; }

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("common_user");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(m => m.Username)
            .HasColumnName("username");

        builder.Property(m => m.LowerUsername)
            .HasColumnName("lower_username");

        builder.Property(m => m.FullName)
            .HasColumnName("full_name");

        builder.Property(m => m.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(m => m.LastLogin)
            .HasColumnName("last_login");

        builder.Property(m => m.PasswordHash)
            .HasColumnName("password_hash");

        builder.Property(m => m.Salt)
            .HasColumnName("salt");

        builder.Property(m => m.Email)
            .HasColumnName("email");

        builder.Property(m => m.LowerEmail)
            .HasColumnName("lower_email");
    }
}
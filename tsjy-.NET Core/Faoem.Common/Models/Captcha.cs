using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faoem.Common.Models;

[EntityTypeConfiguration(typeof(Captcha))]
public class Captcha : IEntityTypeConfiguration<Captcha>
{
    private string _email = null!;

    public Guid Id { get; private set; }
    [MaxLength(256)] public string LowerEmail { get; private set; } = null!;

    [MaxLength(256)]
    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            LowerEmail = value.ToLower();
        }
    }

    public int Code { get; set; }
    public long ExpireTime { get; set; }

    public void Configure(EntityTypeBuilder<Captcha> builder)
    {
        builder.ToTable("common_captcha");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(m => m.Email)
            .HasColumnName("email");

        builder.Property(m => m.LowerEmail)
            .HasColumnName("lower_email");

        builder.Property(m => m.Code)
            .HasColumnName("code");

        builder.Property(m => m.ExpireTime)
            .HasColumnName("expire_time");
    }
}
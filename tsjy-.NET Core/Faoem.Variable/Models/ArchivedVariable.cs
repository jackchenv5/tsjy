using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faoem.Variable.Models;

public enum ArchiveType
{
    /// <summary>
    /// 系统归档。由其他模块添加的归档，只能由添加数据的模块进行删除。例如：电机模块对电机电流进行归档，只能由电机模块删除。
    /// </summary>
    System = 0,

    /// <summary>
    /// 用户归档。
    /// </summary>
    User = 1
}

public enum ArchiveMode
{
    /// <summary>
    /// 变化时归档。
    /// </summary>
    Change = 0,

    /// <summary>
    /// 周期归档。
    /// </summary>
    Period = 1
}

[EntityTypeConfiguration(typeof(ArchivedVariable))]
public class ArchivedVariable : IEntityTypeConfiguration<ArchivedVariable>
{
    public long Id { get; set; }

    [MaxLength(128)] public string ConnectorInstance { get; set; } = null!;
    [MaxLength(128)] public string ConnectionName { get; set; } = null!;
    [MaxLength(128)] public string DataPoint { get; set; } = null!;
    [MaxLength(256)] public string Name { get; set; } = null!;
    [MaxLength(32)] public string DataType { get; set; } = null!;

    /// <summary>
    /// 0 - 系统归档。由其他模块添加的归档，只能由添加数据的模块进行删除。例如：电机模块对电机电流进行归档，只能由电机模块删除。<br></br>
    /// 1 - 用户归档。
    /// </summary>
    public ArchiveType ArchiveType { get; set; }

    /// <summary>
    /// 归档模式。<br/>
    /// 0 - 变化时归档。<br/>
    /// 1 - 周期归档。<br/>
    /// </summary>
    public ArchiveMode ArchiveMode { get; set; }

    /// <summary>
    /// 归档间隔。单位：秒。仅当 <see cref="ArchiveMode"/> 为 1 时生效。<br/>
    /// 当数值大于 0 时，表示定时归档的间隔；当数值小于等于 0 时，将跳过归档。
    /// </summary>
    public int ArchiveInterval { get; set; }

    /// <summary>
    /// 由哪个模块创建。
    /// </summary>
    [MaxLength(128)]
    public string CreatedBy { get; set; } = null!;

    public long CreatedAt { get; set; }

    public void Configure(EntityTypeBuilder<ArchivedVariable> builder)
    {
        builder.ToTable("variable_archived_variable");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(m => m.ConnectorInstance)
            .HasColumnName("connector_instance");

        builder.Property(m => m.ConnectionName)
            .HasColumnName("connection_name");

        builder.Property(m => m.DataPoint)
            .HasColumnName("data_point");

        builder.Property(m => m.Name)
            .HasColumnName("name");

        builder.Property(m => m.DataType)
            .HasColumnName("data_type");


        builder.Property(m => m.ArchiveType)
            .HasColumnName("archive_type");

        builder.Property(m => m.ArchiveMode)
            .HasColumnName("archive_mode");

        builder.Property(m => m.ArchiveInterval)
            .HasColumnName("archive_interval");
        
        builder.Property(m=>m.CreatedBy)
            .HasColumnName("created_by");
        
        builder.Property(m=>m.CreatedAt)
            .HasColumnName("created_at");
    }
}
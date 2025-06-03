using Microsoft.EntityFrameworkCore;
using Tsjy.Models;

namespace Tsjy.DbContexts;

public class TsjyDbContext(IConfiguration configuration) : DbContext
{
    protected readonly IConfiguration Configuration = configuration;

    public DbSet<TsjyStatusBinding> StatusBindings { get; set; } = null!;
    public DbSet<TsjyAlarmDefinition> AlarmDefinitions { get; set; } = null!;
    public DbSet<TsjyAlarmHistory> AlarmHistories { get; set; } = null!;
    public DbSet<TsjyMotor> Motors { get; set; } = null!;
    public DbSet<TsjyMotorBinding> MotorBindings { get; set; } = null!;
    public DbSet<TsjyProductionBinding> ProductionBindings { get; set; } = null!;
    public DbSet<TsjyProductionData> ProductionData { get; set; } = null!;
    public DbSet<TsjyCraftBinding> CraftBindings { get; set; } = null!;
    public DbSet<TsjyCraftData> CraftData { get; set; } = null!;
    public DbSet<TsjyPart> Parts { get; set; } = null!;
    public DbSet<TsjyPartMaintainHistory> PartMaintainHistories { get; set; } = null!;
    public DbSet<RollDiameter> RollDiameters { get; set; }
}
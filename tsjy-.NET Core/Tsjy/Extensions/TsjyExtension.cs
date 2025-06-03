using Faoem.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.AspNetCore;
using Tsjy.DbContexts;
using Tsjy.Jobs;
using Tsjy.Services;

namespace Tsjy.Extensions;

public static class TsjyExtension
{
    public static IServiceCollection AddTsjy(this IServiceCollection services, IConfiguration configuration)
    {
        // 数据库
        services.AddSqliteDbContext<TsjyDbContext, SqliteTsjyDbContext>(configuration);

        // 状态变量绑定
        services.AddSingleton<StatusBindingService>();

        // 状态记录
        services.AddSingleton<StatusRecordService>();

        // 报警记录
        services.AddSingleton<AlarmRecordService>();

        // 报警定义
        services.AddScoped<AlarmDefinitionService>();

        // 报警
        services.AddScoped<AlarmService>();

        // 电机
        services.AddScoped<MotorService>();

        // 电机绑定
        services.AddSingleton<MotorBindingService>();

        // 电机数据
        services.AddSingleton<MotorRecordService>();

        // 生产数据绑定
        services.AddSingleton<ProductionBindingService>();

        // 生产数据记录
        services.AddSingleton<ProductionRecordService>();

        // 工艺绑定
        services.AddSingleton<CraftBindingService>();

        // 工艺数据记录
        services.AddSingleton<CraftRecordService>();

        // 易损件记录
        services.AddSingleton<PartRecordService>();

        // 切割机单例服务
        services.AddSingleton<TsjySingleService>();

        // 定时任务
        services.AddQuartz(configurator =>
        {
            // 电机每秒归档
            configurator
                .AddJob<MotorJob>(jobConfigurator => jobConfigurator.WithIdentity(MotorJob.JobKey))
                .AddTrigger(triggerConfigurator =>
                {
                    triggerConfigurator
                        .ForJob(MotorJob.JobKey)
                        .WithSimpleSchedule(builder => builder.WithIntervalInSeconds(1).RepeatForever());
                });

            // 数据清理
            configurator
                .AddJob<CleanJob>(jobConfigurator => jobConfigurator.WithIdentity(CleanJob.JobKey))
                .AddTrigger(triggerConfigurator =>
                {
                    triggerConfigurator
                        .ForJob(CleanJob.JobKey)
                        .WithSimpleSchedule(builder => builder.WithIntervalInHours(1).RepeatForever());
                });
        });

        services.AddQuartzServer(options => options.WaitForJobsToComplete = true);

        return services;
    }

    public static WebApplication ConfigureTsjy(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var tsjyDbContext = serviceProvider.GetRequiredService<TsjyDbContext>();
        tsjyDbContext.Database.Migrate();

        // 状态记录服务
        serviceProvider.GetRequiredService<StatusRecordService>();

        // 报警记录服务
        serviceProvider.GetRequiredService<AlarmRecordService>();

        // 生产数据记录
        serviceProvider.GetRequiredService<ProductionRecordService>();

        app.ConfigureDefaultMenus();

        return app;
    }
}
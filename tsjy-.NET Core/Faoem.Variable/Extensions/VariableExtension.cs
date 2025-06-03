using Faoem.Common.Extensions;
using Faoem.Variable.DbContexts;
using Faoem.Variable.Jobs;
using Faoem.Variable.Services.Archive;
using Faoem.Variable.Services.InfluxDbClient;
using Faoem.Variable.Services.Variable;
using Faoem.Variable.Services.VariableArchive;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Faoem.Variable.Extensions;

public static class VariableExtension
{
    public static IServiceCollection AddVariable(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSqliteDbContext<VariableDbContext, SqliteVariableDbContext>(configuration);
        services.AddMySqlDbContext<VariableDbContext, MySqlVariableDbContext>(configuration);

        services.AddSingleton<IVariableService, VariableService>();
        services.AddSingleton<IInfluxDbClientService, InfluxDbClientService>();
        services.AddHostedService<ArchiveService>();

        services.AddQuartz(configurator =>
        {
            configurator
                .AddJob<ArchiveJob>(
                    jobConfigurator => jobConfigurator.WithIdentity(ArchiveJob.JobKey)
                )
                .AddTrigger(
                    triggerConfigurator => triggerConfigurator.ForJob(ArchiveJob.JobKey)
                        .WithSimpleSchedule(builder => builder.WithIntervalInSeconds(1).RepeatForever())
                );
        });
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        services.AddSingleton<IVariableArchiveService, VariableArchiveService>();

        return services;
    }

    public static WebApplication ConfigureVariable(this WebApplication app)
    {
        var influxDbClientService = app.Services.GetRequiredService<IInfluxDbClientService>();
            influxDbClientService.TryCreateBucketAsync().Wait();
         
        using var scope = app.Services.CreateScope();
        var provider = scope.ServiceProvider;

        var variableDbContext = provider.GetRequiredService<VariableDbContext>();
        variableDbContext.Database.Migrate();

        app.AddDefaultMenu();

        return app;
    }
}
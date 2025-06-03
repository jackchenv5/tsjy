using Faoem.Common.Extensions;
using Faoem.FacilityStatus.DbContexts;
using Faoem.FacilityStatus.Services.FacilityStatus;
using Faoem.FacilityStatus.Services.StatusBindingService;
using Faoem.FacilityStatus.Services.StatusRecord;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Faoem.FacilityStatus.Extensions;

public static class FacilityStatusExtension
{
    public static IServiceCollection AddFacilityStatus(this IServiceCollection services, IConfiguration configuration)
    {
        // 配置数据库相关服务
        services.AddSqliteDbContext<FacilityStatusDbContext, SqliteFacilityStatusDbContext>(configuration);
        services.AddMySqlDbContext<FacilityStatusDbContext, MySqlFacilityStatusDbContext>(configuration);

        // 设备状态绑定服务
        services.AddSingleton<IStatusBindingService, StatusBindingService>();

        // 状态记录服务
        services.AddHostedService<StatusRecordService>();

        // 设备状态服务
        services.AddScoped<IFacilityStatusService, FacilityStatusService>();

        return services;
    }

    public static WebApplication ConfigureFacilityStatus(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var facilityStatusDbContext = serviceProvider.GetRequiredService<FacilityStatusDbContext>();
        facilityStatusDbContext.Database.Migrate();

        app.AddDefaultMenu();

        return app;
    }
}
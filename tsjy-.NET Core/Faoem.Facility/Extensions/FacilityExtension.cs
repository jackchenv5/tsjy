using Faoem.Common.Extensions;
using Faoem.Facility.DbContexts;
using Faoem.Facility.Services.Facility;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Faoem.Facility.Extensions;

public static class FacilityExtension
{
    public static IServiceCollection AddFacility(this IServiceCollection services, IConfiguration configuration)
    {
        // 数据库相关服务
        services.AddSqliteDbContext<FacilityDbContext, SqliteFacilityDbContext>(configuration);
        services.AddMySqlDbContext<FacilityDbContext, MySqlFacilityDbContext>(configuration);

        // 设备服务
        services.AddScoped<IFacilityService, FacilityService>();

        return services;
    }

    public static WebApplication ConfigureFacility(this WebApplication app)
    {
        // 数据库迁移
        using var scope = app.Services.CreateScope();
        var facilityDbContext = scope.ServiceProvider.GetRequiredService<FacilityDbContext>();
        facilityDbContext.Database.Migrate();

        facilityDbContext.AddSeedData();

        app.AddDefaultMenu();

        return app;
    }
}
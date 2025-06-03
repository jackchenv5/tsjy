using Faoem.Common.Extensions;
using Faoem.Shift.DbContexts;
using Faoem.Shift.Services.Shift;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Faoem.Shift.Extensions;

public static class ShiftExtension
{
    public static IServiceCollection AddShift(this IServiceCollection services, IConfiguration configuration)
    {
        // 数据库相关服务
        services.AddSqliteDbContext<ShiftDbContext, SqliteShiftDbContext>(configuration);
        services.AddMySqlDbContext<ShiftDbContext, MySqlShiftDbContext>(configuration);

        services.AddScoped<IShiftService, ShiftService>();

        return services;
    }

    public static WebApplication ConfigureShift(this WebApplication app)
    {
        // 数据库迁移
        using var scope = app.Services.CreateScope();
        var shiftDbContext = scope.ServiceProvider.GetRequiredService<ShiftDbContext>();
        shiftDbContext.Database.Migrate();

        shiftDbContext.AddSeedData();

        app.AddDefaultMenu();

        return app;
    }
}
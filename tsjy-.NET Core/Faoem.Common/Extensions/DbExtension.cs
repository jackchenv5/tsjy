using Faoem.Common.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Faoem.Common.Extensions;

public static class DbExtension
{
    public static IServiceCollection AddSqliteDbContext<TDbContext, TDbContextImplementation>(
        this IServiceCollection services,
        IConfiguration configuration
    )
        where TDbContextImplementation : DbContext, TDbContext
    {
        // 配置 sqlite 数据库服务
        var dbOptions = configuration.GetSection("Db").Get<DbOptions>();
        if (dbOptions?.Type == "Sqlite")
        {
            services.AddDbContext<TDbContext, TDbContextImplementation>();
        }

        return services;
    }

    public static IServiceCollection AddMySqlDbContext<TDbContext, TDbContextImplementation>(
        this IServiceCollection services,
        IConfiguration configuration
    )
        where TDbContextImplementation : DbContext, TDbContext
    {
        // 配置 mysql 数据库服务
        var dbOptions = configuration.GetSection("Db").Get<DbOptions>();
        if (dbOptions?.Type == "MySql")
        {
            services.AddDbContext<TDbContext, TDbContextImplementation>();
        }

        return services;
    }
}
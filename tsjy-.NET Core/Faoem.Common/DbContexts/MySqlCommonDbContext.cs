using Faoem.Common.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Faoem.Common.DbContexts;

internal class MySqlCommonDbContext(IConfiguration configuration) : CommonDbContext(configuration)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbOptions = Configuration.GetSection("Db").Get<DbOptions>();
        var version = ServerVersion.AutoDetect(dbOptions?.MySqlConnectionString);
        optionsBuilder.UseMySql(dbOptions?.MySqlConnectionString, version);
    }
}
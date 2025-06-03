using Faoem.Common.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Faoem.Common.DbContexts;

internal class SqliteCommonDbContext(IConfiguration configuration) : CommonDbContext(configuration)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbOptions = Configuration.GetSection("Db").Get<DbOptions>();
        optionsBuilder.UseSqlite(dbOptions?.SqliteConnectionString);
    }
}
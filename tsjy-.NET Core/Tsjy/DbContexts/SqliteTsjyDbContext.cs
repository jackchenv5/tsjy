using Faoem.Common.Options;
using Microsoft.EntityFrameworkCore;

namespace Tsjy.DbContexts;

public class SqliteTsjyDbContext(IConfiguration configuration) : TsjyDbContext(configuration)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var dbOptions = Configuration.GetSection("Db").Get<DbOptions>();
        optionsBuilder.UseSqlite(dbOptions?.SqliteConnectionString);
    }
}
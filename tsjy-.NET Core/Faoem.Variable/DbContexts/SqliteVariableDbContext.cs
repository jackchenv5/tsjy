using Faoem.Common.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Faoem.Variable.DbContexts;

internal class SqliteVariableDbContext(IConfiguration configuration) : VariableDbContext(configuration)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbOptions = Configuration.GetSection("Db").Get<DbOptions>();
        optionsBuilder.UseSqlite(dbOptions?.SqliteConnectionString);
    }
}
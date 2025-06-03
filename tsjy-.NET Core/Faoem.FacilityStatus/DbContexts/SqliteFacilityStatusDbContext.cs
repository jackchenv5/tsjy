using Faoem.Common.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Faoem.FacilityStatus.DbContexts;

internal class SqliteFacilityStatusDbContext(IConfiguration configuration) : FacilityStatusDbContext(configuration)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var dbOptions = Configuration.GetSection("Db").Get<DbOptions>();
        optionsBuilder.UseSqlite(dbOptions?.SqliteConnectionString);
    }
}
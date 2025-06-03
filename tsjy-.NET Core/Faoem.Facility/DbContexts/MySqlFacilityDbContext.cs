using Faoem.Common.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Faoem.Facility.DbContexts;

internal class MySqlFacilityDbContext(IConfiguration configuration) : FacilityDbContext(configuration)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var dbOptions = Configuration.GetSection("Db").Get<DbOptions>();
        var version = ServerVersion.AutoDetect(dbOptions?.MySqlConnectionString);
        optionsBuilder.UseMySql(dbOptions?.MySqlConnectionString, version);
    }
}
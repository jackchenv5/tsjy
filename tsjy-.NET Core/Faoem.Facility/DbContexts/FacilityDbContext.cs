using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Faoem.Facility.DbContexts;

internal class FacilityDbContext(IConfiguration configuration) : DbContext
{
    protected readonly IConfiguration Configuration = configuration;

    public DbSet<Models.Facility> Facilities { get; set; }
}
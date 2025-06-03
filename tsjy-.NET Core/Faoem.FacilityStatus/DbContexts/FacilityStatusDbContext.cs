using Faoem.FacilityStatus.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Faoem.FacilityStatus.DbContexts;

internal class FacilityStatusDbContext(IConfiguration configuration) : DbContext
{
    protected readonly IConfiguration Configuration = configuration;

    public DbSet<StatusBinding> VariableBindings { get; set; } = null!;
}
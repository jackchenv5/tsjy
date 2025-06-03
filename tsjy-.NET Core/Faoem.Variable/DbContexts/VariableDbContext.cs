using Faoem.Variable.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Faoem.Variable.DbContexts;

internal class VariableDbContext(IConfiguration configuration) : DbContext
{
    protected readonly IConfiguration Configuration = configuration;

    public DbSet<ArchivedVariable> ArchivedVariables { get; set; }
}
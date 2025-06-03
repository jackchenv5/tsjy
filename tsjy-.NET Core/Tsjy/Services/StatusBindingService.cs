using Faoem.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Tsjy.DbContexts;
using Tsjy.Models;

namespace Tsjy.Services;

public class StatusBindingService
{
    // singleton

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private List<TsjyStatusBinding> _statusBindings = [];

    public StatusBindingService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;

        RefreshBindingList();
    }

    public async Task<List<TsjyStatusBinding>> GetStatusBindingsAsync()
    {
        return await Task.FromResult(_statusBindings);
    }

    public async Task<List<TsjyStatusBinding>> GetStatusBindingsAsync(long facilityId)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var tsjyDbContext = serviceProvider.GetRequiredService<TsjyDbContext>();

        var types = Enum.GetValues<StatusBindingType>().ToList();

        var bindings = new List<TsjyStatusBinding>();
        foreach (var type in types)
        {
            var binding = await tsjyDbContext.StatusBindings
                .Where(b => b.FacilityId == facilityId)
                .FirstOrDefaultAsync(b => b.BindingType == type);
            if (binding is null)
            {
                binding = new TsjyStatusBinding
                {
                    BindingType = type,
                    ConnectorInstance = string.Empty,
                    ConnectionName = string.Empty,
                    DataPoint = string.Empty,
                    Name = string.Empty,
                    FacilityId = facilityId
                };
                await tsjyDbContext.StatusBindings.AddAsync(binding);
            }

            bindings.Add(binding);
        }

        await tsjyDbContext.SaveChangesAsync();

        return bindings;
    }

    public async Task UpdateStatusBindingAsync(TsjyStatusBinding tsjyStatusBinding)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var tsjyDbContext = serviceProvider.GetRequiredService<TsjyDbContext>();

        var binding = await tsjyDbContext.StatusBindings.FindAsync(tsjyStatusBinding.Id);
        if (binding is null)
        {
            throw new AppException("Status binding not found", 404);
        }

        binding.ConnectorInstance = tsjyStatusBinding.ConnectorInstance;
        binding.ConnectionName = tsjyStatusBinding.ConnectionName;
        binding.DataPoint = tsjyStatusBinding.DataPoint;
        binding.Name = tsjyStatusBinding.Name;
        await tsjyDbContext.SaveChangesAsync();
        RefreshBindingList();
    }

    private void RefreshBindingList()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var tsjyDbContext = serviceProvider.GetRequiredService<TsjyDbContext>();

        _statusBindings = tsjyDbContext.StatusBindings.ToList();
    }
}
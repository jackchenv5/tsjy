using Faoem.Common.Exceptions;
using Faoem.FacilityStatus.DbContexts;
using Faoem.FacilityStatus.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faoem.FacilityStatus.Services.StatusBindingService;

internal class StatusBindingService : IStatusBindingService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private List<StatusBinding> _statusBindings = [];

    public StatusBindingService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;

        RefreshBindingList();
    }

    public async Task<List<StatusBinding>> GetStatusBindingsAsync(long facilityId)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var facilityStatusDbContext = serviceProvider.GetRequiredService<FacilityStatusDbContext>();

        var types = new List<BindingType>
        {
            BindingType.Running,
            BindingType.Standby,
            BindingType.Stopped,
            BindingType.Error
        };

        var bindings = new List<StatusBinding>();
        foreach (var type in types)
        {
            var binding = await facilityStatusDbContext.VariableBindings
                .Where(b => b.FacilityId == facilityId)
                .FirstOrDefaultAsync(b => b.BindingType == type);
            if (binding is null)
            {
                binding = new StatusBinding
                {
                    BindingType = type,
                    ConnectorInstance = string.Empty,
                    ConnectionName = string.Empty,
                    DataPoint = string.Empty,
                    Name = string.Empty,
                    FacilityId = facilityId
                };
                await facilityStatusDbContext.VariableBindings.AddAsync(binding);
            }

            bindings.Add(binding);
        }

        await facilityStatusDbContext.SaveChangesAsync();

        return bindings;
    }

    public async Task<List<StatusBinding>> GetStatusBindingsAsync()
    {
        return await Task.FromResult(_statusBindings);
    }

    public async Task UpdateStatusBindingAsync(StatusBinding statusBinding)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var facilityStatusDbContext = serviceProvider.GetRequiredService<FacilityStatusDbContext>();

        var binding = await facilityStatusDbContext.VariableBindings.FindAsync(statusBinding.Id);
        if (binding is null)
        {
            throw new AppException("Status binding not found", 404);
        }

        binding.ConnectorInstance = statusBinding.ConnectorInstance;
        binding.ConnectionName = statusBinding.ConnectionName;
        binding.DataPoint = statusBinding.DataPoint;
        binding.Name = statusBinding.Name;
        await facilityStatusDbContext.SaveChangesAsync();
        RefreshBindingList();
    }

    private void RefreshBindingList()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var facilityStatusDbContext = serviceProvider.GetRequiredService<FacilityStatusDbContext>();
        _statusBindings = facilityStatusDbContext.VariableBindings.ToList();
    }
}
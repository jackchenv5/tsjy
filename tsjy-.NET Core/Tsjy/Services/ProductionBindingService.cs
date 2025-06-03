using Faoem.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Tsjy.DbContexts;
using Tsjy.Eunms;
using Tsjy.Models;

namespace Tsjy.Services;

public class ProductionBindingService
{
    // Singleton
    // 使用单例服务以保存绑定状态，避免每次变量变化都要查询数据库

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private List<TsjyProductionBinding> _productionBindings = [];

    public ProductionBindingService(
        IServiceScopeFactory serviceScopeFactory
    )
    {
        _serviceScopeFactory = serviceScopeFactory;

        RefreshBindingList();
    }

    private void RefreshBindingList()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var tsjyDbContext = scope.ServiceProvider.GetRequiredService<TsjyDbContext>();
        _productionBindings = tsjyDbContext.ProductionBindings.ToList();
    }

    public async Task<List<TsjyProductionBinding>> GetProductionBindingsAsync()
    {
        return await Task.FromResult(_productionBindings);
    }

    public async Task<List<TsjyProductionBinding>> GetProductionBindingsAsync(long facilityId)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var tsjyDbContext = scope.ServiceProvider.GetRequiredService<TsjyDbContext>();
        var types = Enum.GetValues<ProductionBinding>().ToList();
        var bindings = new List<TsjyProductionBinding>();
        foreach (var type in types)
        {
            var binding = await tsjyDbContext.ProductionBindings
                .Where(productionBinding => productionBinding.FacilityId == facilityId)
                .FirstOrDefaultAsync(productionBinding => productionBinding.BindingType == type);
            if (binding is null)
            {
                binding = new TsjyProductionBinding()
                {
                    BindingType = type,
                    ConnectorInstance = string.Empty,
                    ConnectionName = string.Empty,
                    DataPoint = string.Empty,
                    Name = string.Empty,
                    FacilityId = facilityId
                };
                await tsjyDbContext.ProductionBindings.AddAsync(binding);
            }

            bindings.Add(binding);
        }

        await tsjyDbContext.SaveChangesAsync();
        return bindings;
    }

    public async Task UpdateProductionBindingAsync(TsjyProductionBinding updateProductionBinding)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var tsjyDbContext = scope.ServiceProvider.GetRequiredService<TsjyDbContext>();
        var binding = await tsjyDbContext.ProductionBindings.FindAsync(updateProductionBinding.Id);
        if (binding is null)
        {
            throw new AppException("Production binding not found", 404);
        }

        binding.ConnectorInstance = updateProductionBinding.ConnectorInstance;
        binding.ConnectionName = updateProductionBinding.ConnectionName;
        binding.DataPoint = updateProductionBinding.DataPoint;
        binding.Name = updateProductionBinding.Name;
        await tsjyDbContext.SaveChangesAsync();
        RefreshBindingList();
    }
}
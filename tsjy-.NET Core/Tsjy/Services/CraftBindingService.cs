using Faoem.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Tsjy.DbContexts;
using Tsjy.Dtos;
using Tsjy.Eunms;
using Tsjy.Models;

namespace Tsjy.Services;

public class CraftBindingService
{
    // singleton
    // 单例服务保留状态，避免多次查询数据库

    private readonly IServiceScopeFactory _serviceScopeFactory;

    private List<TsjyCraftBinding> _craftBindings = [];

    public CraftBindingService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        RefreshBindingList();
    }

    public async Task<List<TsjyCraftBinding>> GetCraftBindingsAsync()
    {
        return await Task.FromResult(_craftBindings);
    }

    public async Task AddCraftGroupAsync(long facilityId)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TsjyDbContext>();
        var q = dbContext.CraftBindings
            .Where(binding => binding.FacilityId == facilityId);
        var qIndex = q.Select(binding => binding.Index).Distinct();

        var count = await qIndex.CountAsync();

        var lastIndex = 0;
        if (count > 0)
        {
            lastIndex = await qIndex.MaxAsync();
        }

        var nextIndex = lastIndex + 1;
        var types = Enum.GetValues<CraftBinding>().ToList();
        foreach (var type in types)
        {
            var binding = new TsjyCraftBinding()
            {
                FacilityId = facilityId,
                BindingType = type,
                Index = nextIndex,
                ConnectorInstance = string.Empty,
                ConnectionName = string.Empty,
                DataPoint = string.Empty,
                Name = string.Empty
            };
            await dbContext.CraftBindings.AddAsync(binding);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteCraftGroupAsync(long facilityId)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TsjyDbContext>();
        var q = dbContext.CraftBindings
            .Where(binding => binding.FacilityId == facilityId);

        var lastIndex = await q
            .Select(binding => binding.Index)
            .Distinct()
            .MaxAsync();
        var bindings = q
            .Where(binding => binding.Index == lastIndex);
        dbContext.CraftBindings.RemoveRange(bindings);
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<CraftBindingDto>> GetCraftBindingsAsync(long facilityId)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TsjyDbContext>();
        var q = dbContext.CraftBindings
            .Where(binding => binding.FacilityId == facilityId);
        var indexes = q
            .Select(binding => binding.Index)
            .Distinct()
            .OrderBy(index => index);
        var dtoId = 0;
        var dtos = new List<CraftBindingDto>();
        foreach (var index in indexes)
        {
            var dto = new CraftBindingDto()
            {
                Id = --dtoId, // 递减避免和数据库中的 id 冲突
                FacilityId = facilityId,
                Index = index
            };
            var bindings = await q.Where(binding => binding.Index == index)
                .ToListAsync();
            dto.Children.AddRange(bindings);
            dtos.Add(dto);
        }

        return dtos;
    }

    public async Task UpdateCraftBindingAsync(TsjyCraftBinding updateBinding)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TsjyDbContext>();
        var binding = await dbContext.CraftBindings.FindAsync(updateBinding.Id);
        if (binding is null)
        {
            throw new AppException("Craft binding not found", 404);
        }

        binding.ConnectorInstance = updateBinding.ConnectorInstance;
        binding.ConnectionName = updateBinding.ConnectionName;
        binding.DataPoint = updateBinding.DataPoint;
        binding.Name = updateBinding.Name;
        await dbContext.SaveChangesAsync();
        RefreshBindingList();
    }

    private void RefreshBindingList()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TsjyDbContext>();
        _craftBindings = dbContext.CraftBindings.ToList();
    }
}
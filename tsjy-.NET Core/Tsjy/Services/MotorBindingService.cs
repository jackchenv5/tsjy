using Faoem.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Tsjy.DbContexts;
using Tsjy.Eunms;
using Tsjy.Models;

namespace Tsjy.Services;

public class MotorBindingService
{
    // singleton
    // 使用单例服务以保存电机绑定状态，避免每次变量变化都要查询数据库

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private List<TsjyMotorBinding> _motorBindings = [];

    public MotorBindingService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;

        RefreshBindingList();
    }

    public async Task<List<TsjyMotorBinding>> GetMotorBindingsAsync()
    {
        return await Task.FromResult(_motorBindings);
    }

    public async Task<List<TsjyMotorBinding>> GetMotorBindingsAsync(long motorId)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var tsjyDbContext = serviceProvider.GetRequiredService<TsjyDbContext>();
        var types = Enum.GetValues<MotorBinding>().ToList();
        var bindings = new List<TsjyMotorBinding>();
        foreach (var type in types)
        {
            var binding = await tsjyDbContext.MotorBindings
                .Where(motorBinding => motorBinding.MotorId == motorId)
                .FirstOrDefaultAsync(motorBinding => motorBinding.BindingType == type);
            if (binding is null)
            {
                binding = new TsjyMotorBinding
                {
                    BindingType = type,
                    ConnectorInstance = string.Empty,
                    ConnectionName = string.Empty,
                    DataPoint = string.Empty,
                    Name = string.Empty,
                    MotorId = motorId,
                    Max = 0,
                    Min = 0
                };
                await tsjyDbContext.MotorBindings.AddAsync(binding);
            }

            bindings.Add(binding);
        }

        await tsjyDbContext.SaveChangesAsync();
        return bindings;
    }

    public async Task UpdateMotorBindingAsync(TsjyMotorBinding updateMotorBinding)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var tsjyDbContext = serviceProvider.GetRequiredService<TsjyDbContext>();
        var binding = await tsjyDbContext.MotorBindings.FindAsync(updateMotorBinding.Id);
        if (binding is null)
        {
            throw new AppException("Motor binding not found", 404);
        }

        binding.ConnectorInstance = updateMotorBinding.ConnectorInstance;
        binding.ConnectionName = updateMotorBinding.ConnectionName;
        binding.DataPoint = updateMotorBinding.DataPoint;
        binding.Name = updateMotorBinding.Name;
        binding.Max = updateMotorBinding.Max;
        binding.Min = updateMotorBinding.Min;
        binding.MaxWarning = updateMotorBinding.MaxWarning;
        binding.MinWarning = updateMotorBinding.MinWarning;
        binding.MaxAlarm = updateMotorBinding.MaxAlarm;
        binding.MinAlarm = updateMotorBinding.MinAlarm;
        await tsjyDbContext.SaveChangesAsync();
        RefreshBindingList();
    }

    private void RefreshBindingList()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var tsjyDbContext = serviceProvider.GetRequiredService<TsjyDbContext>();
        _motorBindings = tsjyDbContext.MotorBindings.ToList();
    }
}
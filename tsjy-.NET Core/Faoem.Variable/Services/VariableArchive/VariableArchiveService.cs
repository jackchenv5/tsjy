using Faoem.Common.Dtos;
using Faoem.Common.Exceptions;
using Faoem.Variable.DbContexts;
using Faoem.Variable.Definitions;
using Faoem.Variable.Inputs;
using Faoem.Variable.Models;
using Faoem.Variable.Services.InfluxDbClient;
using Faoem.Variable.Services.Variable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faoem.Variable.Services.VariableArchive;

internal class VariableArchiveService(
    IVariableService variableService,
    IServiceScopeFactory serviceScopeFactory,
    IInfluxDbClientService influxDbClientService
)
    : IVariableArchiveService
{
    private List<ArchivedVariable>? _archivedVariables;

    public async Task<List<ArchivedVariable>> GetArchivedVariablesAsync()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var variableDbContext = scope.ServiceProvider.GetRequiredService<VariableDbContext>();

        return await variableDbContext.ArchivedVariables.ToListAsync();
    }

    public async Task<PagedDto<ArchivedVariable>> GetArchivedVariablesAsync(VariableFilterInput filter)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var variableDbContext = scope.ServiceProvider.GetRequiredService<VariableDbContext>();

        var dto = new PagedDto<ArchivedVariable>
        {
            Total = await variableDbContext.ArchivedVariables.CountAsync(),
            Items = await variableDbContext.ArchivedVariables
                .Where(v =>
                    (
                        string.IsNullOrWhiteSpace(filter.ConnectorInstance) ||
                        v.ConnectorInstance.Contains(filter.ConnectorInstance)
                    ) &&
                    (
                        string.IsNullOrWhiteSpace(filter.ConnectionName) ||
                        v.ConnectionName.Contains(filter.ConnectionName)
                    ) &&
                    (
                        string.IsNullOrWhiteSpace(filter.DataPointName) ||
                        v.DataPoint.Contains(filter.DataPointName)
                    ) &&
                    (
                        string.IsNullOrEmpty(filter.Name) ||
                        v.Name.Contains(filter.Name)
                    )
                )
                .OrderBy(v => v.Id)
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync()
        };

        return dto;
    }

    /// <summary>
    /// 添加归档变量
    /// </summary>
    /// <param name="appVariableGuid">要归档变量的 guid</param>
    /// <param name="createdBy">由哪个模块创建</param>
    /// <param name="archiveType">归档类型：0 - 系统归档，1 - 用户归档</param>
    /// <param name="archiveMode">归档模式：0 - 变化时归档，1 - 周期归档</param>
    /// <returns></returns>
    private async Task<ArchivedVariable?> AddArchivedVariableAsync(
        Guid appVariableGuid,
        string createdBy,
        ArchiveType archiveType = ArchiveType.User,
        ArchiveMode archiveMode = ArchiveMode.Change
    )
    {
        if (await IsArchivedVariableExistAsync(appVariableGuid, archiveMode, createdBy))
        {
            return null;
        }

        using var scope = serviceScopeFactory.CreateScope();
        var variableDbContext = scope.ServiceProvider.GetRequiredService<VariableDbContext>();

        var variable = await variableService.GetVariablesAsync(appVariableGuid);
        var archivedVariable = new ArchivedVariable
        {
            ConnectorInstance = variable.ConnectorInstance,
            ConnectionName = variable.ConnectionName,
            DataPoint = variable.DataPointName,
            Name = variable.Name,
            DataType = variable.DataType,
            ArchiveType = archiveType,
            ArchiveMode = archiveMode,
            ArchiveInterval = 60,
            CreatedBy = createdBy,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        await variableDbContext.ArchivedVariables.AddAsync(archivedVariable);

        await variableDbContext.SaveChangesAsync();

        await RefreshArchivedVariablesAsync();

        return archivedVariable;
    }

    public async Task<ArchivedVariable?> AddUserArchivedAsync(
        Guid appVariableGuid,
        string createdBy,
        ArchiveMode archiveMode = ArchiveMode.Change
    )
    {
        return await AddArchivedVariableAsync(appVariableGuid, createdBy, ArchiveType.User, archiveMode);
    }

    public async Task UpdateArchiveIntervalAsync(long id, int interval)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var variableDbContext = scope.ServiceProvider.GetRequiredService<VariableDbContext>();

        var archivedVariable = await variableDbContext.ArchivedVariables.FindAsync(id);
        if (archivedVariable == null)
        {
            throw new AppException("Archived variable not found.", 404);
        }

        if (archivedVariable.ArchiveMode == ArchiveMode.Change)
        {
            throw new AppException("Change mode archived variable can't update interval.", 400);
        }

        archivedVariable.ArchiveInterval = interval;
        await variableDbContext.SaveChangesAsync();

        await RefreshArchivedVariablesAsync();
    }

    public async Task DeleteArchivedVariableAsync(long id)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var variableDbContext = scope.ServiceProvider.GetRequiredService<VariableDbContext>();

        var archivedVariable = await variableDbContext.ArchivedVariables.FindAsync(id);
        if (archivedVariable == null)
        {
            throw new AppException("Archived variable not found.", 404);
        }

        variableDbContext.ArchivedVariables.Remove(archivedVariable);
        await variableDbContext.SaveChangesAsync();
        await RefreshArchivedVariablesAsync();

        await influxDbClientService.DeleteDataAsync(archivedVariable);
    }

    private async Task RefreshArchivedVariablesAsync()
    {
        _archivedVariables = await GetArchivedVariablesAsync();
    }

    public async Task<bool> IsArchivedOnChangeAsync(AppVariable variable)
    {
        if (_archivedVariables is null)
        {
            await RefreshArchivedVariablesAsync();
        }

        if (_archivedVariables is null)
        {
            return false;
        }

        return _archivedVariables.Any(archivedVariable =>
            archivedVariable.ArchiveMode == 0 &&
            archivedVariable.ConnectorInstance == variable.ConnectorInstance &&
            archivedVariable.ConnectionName == variable.ConnectionName &&
            archivedVariable.DataPoint == variable.DataPointName &&
            archivedVariable.Name == variable.Name
        );
    }

    public async Task<List<ArchivedVariable>> GetIntervalArchivedVariablesAsync()
    {
        if (_archivedVariables is null)
        {
            await RefreshArchivedVariablesAsync();
        }

        if (_archivedVariables is null)
        {
            return [];
        }

        return _archivedVariables.Where(variable =>
            variable is { ArchiveMode: ArchiveMode.Period, ArchiveInterval: > 0 }
        ).ToList();
    }

    public async Task<ArchivedVariable?> AddSystemArchivedAsync(
        Guid appVariableGuid,
        string createdBy,
        ArchiveMode archiveMode = ArchiveMode.Change
    )
    {
        return await AddArchivedVariableAsync(appVariableGuid, createdBy, ArchiveType.System, archiveMode);
    }

    public async Task<ArchivedVariable?> GetArchivedVariableAsync(long id)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var variableDbContext = scope.ServiceProvider.GetRequiredService<VariableDbContext>();

        return await variableDbContext.ArchivedVariables.FindAsync(id);
    }

    private async Task<bool> IsArchivedVariableExistAsync(
        Guid appVariableGuid,
        ArchiveMode archiveMode,
        string createdBy
    )
    {
        var variable = await variableService.GetVariablesAsync(appVariableGuid);
        using var scope = serviceScopeFactory.CreateScope();
        var variableDbContext = scope.ServiceProvider.GetRequiredService<VariableDbContext>();
        var exist = await variableDbContext.ArchivedVariables.AnyAsync(archivedVariable =>
            archivedVariable.ConnectorInstance == variable.ConnectorInstance &&
            archivedVariable.ConnectionName == variable.ConnectionName &&
            archivedVariable.DataPoint == variable.DataPointName &&
            archivedVariable.Name == variable.Name &&
            archivedVariable.ArchiveMode == archiveMode &&
            archivedVariable.CreatedBy == createdBy
        );

        return exist;
    }
}
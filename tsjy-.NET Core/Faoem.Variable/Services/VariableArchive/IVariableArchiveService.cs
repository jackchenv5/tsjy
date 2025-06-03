using Faoem.Common.Dtos;
using Faoem.Variable.Definitions;
using Faoem.Variable.Inputs;
using Faoem.Variable.Models;

namespace Faoem.Variable.Services.VariableArchive;

public interface IVariableArchiveService
{
    internal Task<List<ArchivedVariable>> GetArchivedVariablesAsync();
    internal Task<PagedDto<ArchivedVariable>> GetArchivedVariablesAsync(VariableFilterInput filter);

    /// <summary>
    /// 添加用户归档变量
    /// </summary>
    /// <param name="appVariableGuid">要归档变量的 guid</param>
    /// <param name="createdBy">由哪个模块创建</param>
    /// <param name="archiveMode">归档模式：0 - 变化时归档，1 - 周期归档</param>
    /// <returns></returns>
    internal Task<ArchivedVariable?> AddUserArchivedAsync(
        Guid appVariableGuid,
        string createdBy,
        ArchiveMode archiveMode = ArchiveMode.Change
    );

    internal Task UpdateArchiveIntervalAsync(long id, int interval);
    public Task DeleteArchivedVariableAsync(long id);
    internal Task<bool> IsArchivedOnChangeAsync(AppVariable variable);
    internal Task<List<ArchivedVariable>> GetIntervalArchivedVariablesAsync();

    /// <summary>
    /// 添加用户归档变量
    /// </summary>
    /// <param name="appVariableGuid">要归档变量的 guid</param>
    /// <param name="createdBy">由哪个模块创建</param>
    /// <param name="archiveMode">归档模式：0 - 变化时归档，1 - 周期归档</param>
    /// <returns></returns>
    public Task<ArchivedVariable?> AddSystemArchivedAsync(
        Guid appVariableGuid,
        string createdBy,
        ArchiveMode archiveMode = ArchiveMode.Change
    );

    public Task<ArchivedVariable?> GetArchivedVariableAsync(long id);
}
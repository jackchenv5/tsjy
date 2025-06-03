using Faoem.Common.Dtos;
using Faoem.Variable.Definitions;
using Faoem.Variable.EventArgs;
using Faoem.Variable.Inputs;

namespace Faoem.Variable.Services.Variable;

public interface IVariableService
{
    /// <summary>
    /// 更新连接器状态。
    /// 同一个连接器实例只保留一份状态数据。
    /// </summary>
    /// <param name="connectorStatus">连接器状态</param>
    /// <returns></returns>
    public Task UpdateConnectorStatusAsync(AppConnectorStatus connectorStatus);

    /// <summary>
    /// 获取所有连接器实例的状态。
    /// </summary>
    /// <returns></returns>
    public Task<List<AppConnectorStatus>> GetConnectorStatusAsync();

    /// <summary>
    /// 更新数据点定义。
    /// 同一个连接器实例只保留一份数据点定义数据。
    /// </summary>
    /// <param name="dataPointDefinitions">数据点定义</param>
    /// <returns></returns>
    public Task UpdateDataPointDefinitionAsync(List<AppDataPointDefinition> dataPointDefinitions);

    /// <summary>
    /// 更新变量数据。
    /// </summary>
    /// <param name="variableData"></param>
    /// <returns></returns>
    public Task UpdateVariableDataAsync(List<AppVariableData> variableData);

    public Task<PagedDto<AppVariable>> GetVariablesAsync(VariableFilterInput filter);
    public Task<List<AppVariable>> GetVariablesAsync();
    public Task<AppVariable> GetVariablesAsync(Guid guid);

    public event Func<VariableChangedEventArgs, Task>? VariableChangedAsync;
}
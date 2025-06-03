using Faoem.Common.Dtos;
using Faoem.Common.Exceptions;
using Faoem.Variable.Definitions;
using Faoem.Variable.EventArgs;
using Faoem.Variable.Inputs;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;

namespace Faoem.Variable.Services.Variable;

internal class VariableService : IVariableService
{
    private readonly List<AppConnectorStatus> _connectorStatus = [];
    private readonly List<AppDataPointDefinition> _dataPointDefinitions = [];

    private List<AppVariable> _variables = [];

    public event Func<VariableChangedEventArgs, Task>? VariableChangedAsync;

    public VariableService(IManagedMqttClient mqttClient)
    {
        mqttClient.SubscribeAsync("ie/#", MqttQualityOfServiceLevel.AtLeastOnce);
    }

    public Task UpdateConnectorStatusAsync(AppConnectorStatus connectorStatus)
    {
        _connectorStatus.RemoveAll(s => s.ConnectorInstance == connectorStatus.ConnectorInstance);

        _connectorStatus.Add(connectorStatus);

        return Task.CompletedTask;
    }

    public Task<List<AppConnectorStatus>> GetConnectorStatusAsync()
    {
        return Task.FromResult(_connectorStatus);
    }

    public async Task UpdateDataPointDefinitionAsync(List<AppDataPointDefinition> dataPointDefinitions)
    {
        // 一次调用中，所有定义必定来自同一个连接器实例，因此先清除同一个连接器实例的变量定义
        _dataPointDefinitions.RemoveAll(d => d.ConnectorInstance == dataPointDefinitions[0].ConnectorInstance);

        _dataPointDefinitions.AddRange(dataPointDefinitions);

        await MapVariablesAsync();
    }

    public Task UpdateVariableDataAsync(List<AppVariableData> variableData)
    {
        if (variableData.Count == 0)
        {
            return Task.CompletedTask;
        }

        if (_variables.Count == 0)
        {
            return Task.CompletedTask;
        }

        // 一次调用中，所有数据必定来自同一个数据点，因此连接器实例、连接名、数据点名的判断条件只需要取第一个元素
        var connectorInstance = variableData[0].ConnectorInstance;
        var connectionName = variableData[0].ConnectionName;
        var dataPointName = variableData[0].DataPointName;

        var existVariables = _variables
            .Where(v =>
                v.ConnectorInstance == connectorInstance &&
                v.ConnectionName == connectionName &&
                v.DataPointName == dataPointName
            )
            .Select(v => v)
            .AsQueryable();

        /*
         * 2024-7-18
         * 目前，S7 Connector （S7, S7 Plus 协议）和 OPC UA Connector 都仅支持变量变化时更新，
         * 因此，该服务不在额外处理变化的变量，而是将所有发过来的变量都视为变化的变量
         */

        List<AppVariable> newVariables = [];

        // 但是变量数据可能有多个，需要每个都处理
        foreach (var variable in variableData)
        {
            var appVariable = existVariables.FirstOrDefault(v => v.Id == variable.Id);

            if (appVariable is null)
            {
                // 正常情况下应该不应为空
                continue;
            }

            appVariable.Ts = variable.Ts;
            appVariable.Qc = variable.Qc;
            appVariable.Val = variable.Val;

            newVariables.Add(appVariable);
        }

        VariableChangedAsync?.Invoke(new VariableChangedEventArgs(newVariables));

        return Task.CompletedTask;
    }

    private Task MapVariablesAsync()
    {
        var variables = _dataPointDefinitions
            .Select(dataPointDefinition => new AppVariable(
                dataPointDefinition.AccessMode,
                dataPointDefinition.AcquisitionCycleInMs,
                dataPointDefinition.AcquisitionMode,
                dataPointDefinition.DataType,
                dataPointDefinition.Id,
                dataPointDefinition.Name,
                dataPointDefinition.IsArray,
                dataPointDefinition.ConnectorInstance,
                dataPointDefinition.ConnectionName,
                dataPointDefinition.DataPointName))
            .ToList();

        _variables = variables;

        return Task.CompletedTask;
    }

    public Task<PagedDto<AppVariable>> GetVariablesAsync(
        VariableFilterInput filter
    )
    {
        var filteredVariables = _variables
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
                    v.DataPointName.Contains(filter.DataPointName)
                ) &&
                (
                    string.IsNullOrEmpty(filter.Name) ||
                    v.Name.Contains(filter.Name)
                )
            )
            .Skip((filter.PageIndex - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();

        return Task.FromResult(new PagedDto<AppVariable>
        {
            Items = filteredVariables,
            Total = _variables.Count
        });
    }

    public Task<List<AppVariable>> GetVariablesAsync()
    {
        return Task.FromResult(_variables);
    }

    public Task<AppVariable> GetVariablesAsync(Guid guid)
    {
        var variable = _variables.FirstOrDefault(v => v.Guid == guid);

        if (variable is null)
        {
            throw new AppException("Variable not found", 404);
        }

        return Task.FromResult(variable);
    }
}
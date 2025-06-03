using System.Text;
using System.Text.Json;
using Faoem.ModbusTcpConnector.Definitions;
using Faoem.Variable.Definitions;
using Faoem.Variable.Services.Variable;
using Microsoft.Extensions.Hosting;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;

namespace Faoem.ModbusTcpConnector.Services.ModbusTcpConnector;

public class ModbusTcpConnectorService : IHostedService
{
    private const string ModbusTcpStatusTopicPrefix = "ie/s/j/simatic/v1/mbtcp";
    private const string ModbusTcpMetaDataTopicPrefix = "ie/m/j/simatic/v1/mbtcp";
    private const string ModbusTcpDataTopicPrefix = "ie/d/j/simatic/v1/mbtcp";

    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly IManagedMqttClient _mqttClient;
    private readonly IVariableService _variableService;


    private Status _status = new();
    private MetaData _metadata = new();
    private Data _data = new();

    public ModbusTcpConnectorService(IManagedMqttClient mqttClient, IVariableService variableService)
    {
        _mqttClient = mqttClient;
        _variableService = variableService;

        _mqttClient.ApplicationMessageReceivedAsync += MqttClientOnApplicationMessageReceivedAsync;
    }

    private async Task MqttClientOnApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        var topic = arg.ApplicationMessage.Topic;

        var isStatusMessage = topic.StartsWith(ModbusTcpStatusTopicPrefix);
        var isMetaDataMessage = topic.StartsWith(ModbusTcpMetaDataTopicPrefix);
        var isDataMessage = topic.StartsWith(ModbusTcpDataTopicPrefix);

        if (!isStatusMessage && !isMetaDataMessage && !isDataMessage)
        {
            // 不是 Modbus TCP Connector 的数据
            return;
        }

        var connectorInstance = topic.Split("/")[5];
        var payload = Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment);

        if (isDataMessage)
        {
            // handle data   
            await HandleDataAsync(connectorInstance, topic, payload);
            return;
        }

        if (isStatusMessage)
        {
            // handle status
            await HandleStatusAsync(connectorInstance, payload);
            return;
        }

        if (isMetaDataMessage)
        {
            // handle meta data
            await HandleMetaDataAsync(connectorInstance, payload);
        }
    }

    private async Task HandleMetaDataAsync(string connectorInstance, string payload)
    {
        var metaData = JsonSerializer.Deserialize<MetaData>(payload, _jsonSerializerOptions);

        if (metaData is null)
        {
            return;
        }

        _metadata = metaData;

        var appDataPointDefinitions =
            (from connection in _metadata.Connections
                let connectionName = connection.Name
                from dataPoint in connection.DataPoints
                let dataPointName = dataPoint.Name
                from definition in dataPoint.DataPointDefinitions
                select new AppDataPointDefinition(
                    definition.AccessMode,
                    definition.AcquisitionCycleInMs,
                    string.Empty,
                    definition.DataType,
                    definition.Id,
                    definition.Name,
                    false,
                    connectorInstance,
                    connectionName,
                    dataPointName
                )
            )
            .ToList();

        await _variableService.UpdateDataPointDefinitionAsync(appDataPointDefinitions);

        await RequestCompleteConnectorTagsAsync(connectorInstance);
    }

    private async Task RequestCompleteConnectorTagsAsync(string connectorInstance)
    {
        const string topic = "ie/c/j/simatic/v1/updaterequest";

        var payloadObj = new { Path = connectorInstance };
        var payload = JsonSerializer.Serialize(payloadObj);

        await _mqttClient.EnqueueAsync(topic, payload, MqttQualityOfServiceLevel.AtLeastOnce);
    }

    private async Task HandleStatusAsync(string connectorInstance, string payload)
    {
        var status = JsonSerializer.Deserialize<Status>(payload, _jsonSerializerOptions);

        if (status is null)
        {
            return;
        }

        _status = status;

        var appConnectionStatus = status.Connections
            .Select(connection => new AppConnectionStatus(connection.Name, connection.Status))
            .ToList();

        var appConnectorStatus = new AppConnectorStatus(
            connectorInstance,
            status.Seq,
            status.Ts,
            _status.Connector.Status,
            appConnectionStatus);

        await _variableService.UpdateConnectorStatusAsync(appConnectorStatus);
    }

    private async Task HandleDataAsync(string connectorInstance, string topic, string payload)
    {
        var data = JsonSerializer.Deserialize<Data>(payload, _jsonSerializerOptions);
        if (data is null)
        {
            return;
        }

        _data = data;

        var connectionName = topic.Split("/")[8];
        var dataPointName = topic.Split("/")[9];

        var definitions = _metadata.Connections
            .Where(c => c.Name == connectionName)
            .SelectMany(c => c.DataPoints)
            .Where(p => p.Name == dataPointName)
            .SelectMany(p => p.DataPointDefinitions)
            .ToList();

        var appVariableData = new List<AppVariableData>();

        foreach (var record in _data.Records)
        {
            foreach (var value in record.Vals)
            {
                var definition = definitions.FirstOrDefault(d => d.Id == value.Id);
                if (definition is null)
                {
                    continue;
                }

                var val = await GetValueAsync(definition.DataType, value.Val);
                var appVariable = new AppVariableData(
                    connectorInstance,
                    connectionName,
                    dataPointName,
                    value.Id,
                    (AppQualityCode)value.Qc,
                    record.Ts,
                    val
                );
                appVariableData.Add(appVariable);
            }
        }

        await _variableService.UpdateVariableDataAsync(appVariableData);
    }

    private Task<dynamic> GetValueAsync(string dataType, JsonElement val)
    {
        dynamic? result = null;

        switch (dataType)
        {
            // 二进制数
            case ModbusTcpTypes.Bool:
                result = val.GetByte() == 1;
                break;
            case ModbusTcpTypes.Word:
                result = val.GetUInt16();
                break;

            // 整数
            case ModbusTcpTypes.Int:
                result = val.GetInt16();
                break;
            case ModbusTcpTypes.UInt:
                result = val.GetUInt16();
                break;
            case ModbusTcpTypes.DInt:
                result = val.GetInt32();
                break;
            case ModbusTcpTypes.UDInt:
                result = val.GetUInt32();
                break;
            case ModbusTcpTypes.LInt:
                result = val.GetString();
                break;
            case ModbusTcpTypes.ULInt:
                result = val.GetString();
                break;

            // 浮点数
            case ModbusTcpTypes.Real:
                result = val.GetSingle();
                break;
            case ModbusTcpTypes.LReal:
                result = val.GetDouble();
                break;

            // 字符串
            case ModbusTcpTypes.String:
                result = val.GetString() ?? string.Empty;
                // 移除字符串中的空字符
                result = result.Replace("\0", "");
                break;
        }

        return Task.FromResult<dynamic>(result);
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
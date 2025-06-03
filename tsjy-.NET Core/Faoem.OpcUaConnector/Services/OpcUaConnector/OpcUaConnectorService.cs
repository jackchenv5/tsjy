using System.Text;
using System.Text.Json;
using Faoem.OpcUaConnector.Definitions;
using Faoem.Variable.Definitions;
using Faoem.Variable.Services.Variable;
using Microsoft.Extensions.Hosting;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;

namespace Faoem.OpcUaConnector.Services.OpcUaConnector;

public class OpcUaConnectorService : IHostedService
{
    private const string OpcUaStatusTopicPrefix = "ie/s/j/simatic/v1/opcuac";
    private const string OpcUaMetaDataTopicPrefix = "ie/m/j/simatic/v1/opcuac";
    private const string OpcUaDataTopicPrefix = "ie/d/j/simatic/v1/opcuac";

    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly IManagedMqttClient _mqttClient;
    private readonly IVariableService _variableService;


    private Status _status = new();
    private MetaData _metadata = new();
    private Data _data = new();

    public OpcUaConnectorService(IManagedMqttClient mqttClient, IVariableService variableService)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        _mqttClient = mqttClient;
        _variableService = variableService;

        _mqttClient.ApplicationMessageReceivedAsync += MqttClientOnApplicationMessageReceivedAsync;
    }

    private async Task MqttClientOnApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        var topic = arg.ApplicationMessage.Topic;

        var isStatusMessage = topic.StartsWith(OpcUaStatusTopicPrefix);
        var isMetaDataMessage = topic.StartsWith(OpcUaMetaDataTopicPrefix);
        var isDataMessage = topic.StartsWith(OpcUaDataTopicPrefix);

        if (!isStatusMessage && !isMetaDataMessage && !isDataMessage)
        {
            // 不是 OpcUa Connector 的数据
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
                    definition.AcquisitionMode,
                    definition.DataType,
                    definition.Id,
                    definition.Name,
                    definition.ArrayDimensions is not null &&
                    definition.ValueRank is not null &&
                    definition.ValueRank == 1,
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

        foreach (var value in _data.Vals)
        {
            var definition = definitions.FirstOrDefault(d => d.Id == value.Id);

            if (definition is null)
            {
                continue;
            }

            dynamic val;
            if (definition.ArrayDimensions is not null && definition.ValueRank == 1)
            {
                val = await GetArrayValueAsync(definition.DataType, value.Val);
            }
            else
            {
                val = await GetValueAsync(definition.DataType, value.Val);
            }

            var appVariable = new AppVariableData(
                connectorInstance,
                connectionName,
                dataPointName,
                value.Id,
                (AppQualityCode)value.Qc,
                value.Ts,
                val
            );

            appVariableData.Add(appVariable);
        }

        await _variableService.UpdateVariableDataAsync(appVariableData);
    }

    private Task<dynamic> GetValueAsync(string dataType, JsonElement val)
    {
        dynamic? result = null;

        switch (dataType)
        {
            // 博图 二进制数
            case PortalTypes.Bool:
                result = val.GetByte() == 1;
                break;
            case PortalTypes.Byte:
                result = val.GetByte();
                break;
            case PortalTypes.Word:
                result = val.GetUInt16();
                break;
            case PortalTypes.DWord:
                result = val.GetUInt32();
                break;
            case PortalTypes.LWord:
                result = val.GetString();
                break;
            // 博图 整数
            case PortalTypes.SInt:
                result = val.GetSByte();
                break;
            case PortalTypes.USInt:
                result = val.GetByte();
                break;
            case PortalTypes.Int:
                result = val.GetInt16();
                break;
            case PortalTypes.UInt:
                result = val.GetUInt16();
                break;
            case PortalTypes.DInt:
                result = val.GetInt32();
                break;
            case PortalTypes.UDInt:
                result = val.GetUInt32();
                break;
            case PortalTypes.LInt:
                result = val.GetString();
                break;
            case PortalTypes.ULInt:
                result = val.GetString();
                break;
            // 博图 浮点数
            case PortalTypes.Real:
                result = val.GetSingle();
                break;
            case PortalTypes.LReal:
                result = val.GetDouble();
                break;
            // 博图 字符串
            case PortalTypes.Char:
                result = val.GetString();
                break;
            case PortalTypes.String:
                var encodedStr = val.GetString();
                if (string.IsNullOrEmpty(encodedStr))
                {
                    result = "";
                }
                else
                {
                    // 解码
                    var bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(encodedStr);
                    var decodedStr = Encoding.GetEncoding("GBK").GetString(bytes);
                    result = decodedStr;
                }

                break;
        }

        return Task.FromResult<dynamic>(result);
    }

    private async Task<dynamic> GetArrayValueAsync(string dataType, JsonElement val)
    {
        List<dynamic> list = [];

        foreach (var jsonElement in val.EnumerateArray())
        {
            var r = await GetValueAsync(dataType, jsonElement);
            list.Add(r);
        }

        dynamic result = list;

        return result;
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
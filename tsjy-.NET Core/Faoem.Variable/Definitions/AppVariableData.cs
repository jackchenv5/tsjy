namespace Faoem.Variable.Definitions;

public class AppVariableData(
    string connectorInstance,
    string connectionName,
    string dataPointName,
    string id,
    AppQualityCode qc,
    DateTimeOffset ts,
    dynamic val
)
{
    public string ConnectorInstance { get; set; } = connectorInstance;
    public string ConnectionName { get; set; } = connectionName;
    public string DataPointName { get; set; } = dataPointName;

    #region 已知 S7 Connector 和 OPC UA Connector 相同的定义

    public string Id { get; set; } = id;
    public AppQualityCode Qc { get; set; } = qc;
    public DateTimeOffset Ts { get; set; } = ts;
    public dynamic Val { get; set; } = val;

    #endregion
}
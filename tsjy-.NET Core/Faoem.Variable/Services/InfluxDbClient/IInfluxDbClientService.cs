using Faoem.Variable.Definitions;
using Faoem.Variable.Models;
using InfluxDB.Client.Core.Flux.Domain;

namespace Faoem.Variable.Services.InfluxDbClient;

public interface IInfluxDbClientService
{
    public Task TryCreateBucketAsync();

    public Task WriteDataAsync(AppVariable variable, ArchiveMode archiveMode);
    public Task WriteDataAsync(List<AppVariable> variables, ArchiveMode archiveMode);
    public Task WriteDataAsync(List<(string name, string value)> tags, (string name, byte value) field);
    public Task WriteDataAsync(List<(string name, string value)> tags, (string name, float value) field);
    public Task WriteDataAsync(List<(string name, string value)> tags, (string name, double value) field);
    public Task WriteDataAsync(List<(string name, string value)> tags, (string name, decimal value) field);
    public Task WriteDataAsync(List<(string name, string value)> tags, (string name, long value) field);
    public Task WriteDataAsync(List<(string name, string value)> tags, (string name, ulong value) field);
    public Task WriteDataAsync(List<(string name, string value)> tags, (string name, uint value) field);
    public Task WriteDataAsync(List<(string name, string value)> tags, (string name, string value) field);
    public Task WriteDataAsync(List<(string name, string value)> tags, (string name, bool value) field);
    public Task WriteDataAsync(List<(List<(string name, string value)>, (string name, byte value))> points);
    public Task WriteDataAsync(List<(List<(string name, string value)>, (string name, float value))> points);
    public Task WriteDataAsync(List<(List<(string name, string value)>, (string name, double value))> points);
    public Task WriteDataAsync(List<(List<(string name, string value)>, (string name, decimal value))> points);
    public Task WriteDataAsync(List<(List<(string name, string value)>, (string name, long value))> points);
    public Task WriteDataAsync(List<(List<(string name, string value)>, (string name, ulong value))> points);
    public Task WriteDataAsync(List<(List<(string name, string value)>, (string name, uint value))> points);
    public Task WriteDataAsync(List<(List<(string name, string value)>, (string name, string value))> points);
    public Task WriteDataAsync(List<(List<(string name, string value)>, (string name, bool value))> points);
    public Task DeleteDataAsync(ArchivedVariable archivedVariable);
    public Task DeleteDataAsync(List<ArchivedVariable> archivedVariables);

    /// <summary>
    /// 获取指定归档变量的数据
    /// </summary>
    /// <param name="archivedVariable">归档变量</param>
    /// <param name="relative">
    /// 相对时间
    /// <list type="bullet">
    /// <item>1s - 1 second</item>
    /// <item>1m - 1 minute</item>
    /// <item>1h - 1 hour</item>
    /// <item>1d - 1 day</item>
    /// <item>1w - 1 week</item>
    /// <item>1mo - 1 calendar month</item>
    /// <item>1y - 1 calendar year</item>
    /// </list>
    /// <example>
    /// <code>
    /// // 查询 1 个月内的数据
    /// QueryDataAsync(archivedVariable, "-1mo");
    /// // 查询 3 天内的数据
    /// QueryDataAsync(archivedVariable, "-3d");
    /// </code>
    /// </example>
    /// <remarks>
    /// 不能以 0 开始
    /// <list type="bullet">
    /// <item>01m -> 1m</item>
    /// <item>02h05m -> 2h5m</item>
    /// </list>
    /// </remarks>
    /// </param>
    /// <returns></returns>
    public Task<List<FluxTable>> QueryDataAsync(ArchivedVariable archivedVariable, string relative);

    /// <summary>
    /// 获取指定归档变量的数据
    /// </summary>
    /// <param name="archivedVariables">归档变量列表</param>
    /// <param name="relative">
    /// 相对时间
    /// <list type="bullet">
    /// <item>1s - 1 second</item>
    /// <item>1m - 1 minute</item>
    /// <item>1h - 1 hour</item>
    /// <item>1d - 1 day</item>
    /// <item>1w - 1 week</item>
    /// <item>1mo - 1 calendar month</item>
    /// <item>1y - 1 calendar year</item>
    /// </list>
    /// <example>
    /// <code>
    /// // 查询 1 个月内的数据
    /// QueryDataAsync(archivedVariable, "-1mo");
    /// // 查询 3 天内的数据
    /// QueryDataAsync(archivedVariable, "-3d");
    /// </code>
    /// </example>
    /// <remarks>
    /// 不能以 0 开始
    /// <list type="bullet">
    /// <item>01m -> 1m</item>
    /// <item>02h05m -> 2h5m</item>
    /// </list>
    /// </remarks>
    /// </param>
    /// <returns></returns>
    public Task<List<FluxTable>> QueryDataAsync(List<ArchivedVariable> archivedVariables, string relative);

    /// <summary>
    /// 获取指定归档变量的数据
    /// </summary>
    /// <param name="archivedVariable">归档变量</param>
    /// <param name="start">
    /// 开始时间，格式为 "yyyy-MM-ddTHH:mm:ssZ"
    /// </param>
    /// <param name="stop">
    /// 结束时间，格式为 "yyyy-MM-ddTHH:mm:ssZ"
    /// </param>
    /// <returns></returns>
    public Task<List<FluxTable>> QueryDataAsync(ArchivedVariable archivedVariable, string start, string stop);

    /// <summary>
    /// 获取指定归档变量的数据
    /// </summary>
    /// <param name="archivedVariables">归档变量列表</param>
    /// <param name="start">
    /// 开始时间，格式为 "yyyy-MM-ddTHH:mm:ssZ"
    /// </param>
    /// <param name="stop">
    /// 结束时间，格式为 "yyyy-MM-ddTHH:mm:ssZ"
    /// </param>
    /// <returns></returns>
    public Task<List<FluxTable>> QueryDataAsync(List<ArchivedVariable> archivedVariables, string start, string stop);

    /// <summary>
    /// 根据标签查询数据
    /// </summary>
    /// <param name="tags"></param>
    /// <param name="relative"></param>
    /// <returns></returns>
    public Task<List<FluxTable>> QueryDataAsync(List<(string name, string value)> tags, string relative);

    /// <summary>
    /// 根据标签查询数据
    /// </summary>
    /// <param name="tags"></param>
    /// <param name="start"></param>
    /// <param name="stop"></param>
    /// <returns></returns>
    public Task<List<FluxTable>> QueryDataAsync(List<(string name, string value)> tags, string start, string stop);

    /// <summary>
    /// 根据标签查询多组数据
    /// </summary>
    /// <param name="tags"></param>
    /// <param name="relative"></param>
    /// <returns></returns>
    public Task<List<FluxTable>> QueryDataAsync(List<List<(string name, string value)>> tags, string relative);

    /// <summary>
    /// 根据标签查询多组数据
    /// </summary>
    /// <param name="tags"></param>
    /// <param name="start"></param>
    /// <param name="stop"></param>
    /// <returns></returns>
    public Task<List<FluxTable>>
        QueryDataAsync(List<List<(string name, string value)>> tags, string start, string stop);
}
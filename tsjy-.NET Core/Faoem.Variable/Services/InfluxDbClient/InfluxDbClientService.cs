using System.Threading.Channels;
using Faoem.Variable.Definitions;
using Faoem.Variable.Models;
using Faoem.Variable.Options;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core.Flux.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Faoem.Variable.Services.InfluxDbClient;

internal class InfluxDbClientService : IInfluxDbClientService, IDisposable
{
    private readonly InfluxDBClient _client;

    private readonly string _org;
    private readonly string _bucket;
    private readonly string _measurement;
    private readonly Channel<PointData> _channel;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly ILogger<InfluxDbClientService> _logger;


    public InfluxDbClientService(IConfiguration configuration, ILogger<InfluxDbClientService> logger)
    {
        _logger = logger;

        var options = configuration.GetSection("InfluxDb").Get<InfluxDbOptions>() ?? new InfluxDbOptions();

        _org = options.Org;
        _bucket = options.Bucket;
        _measurement = options.Measurement;

        _client = new InfluxDBClient(options.Url, options.Token);
        _channel = Channel.CreateUnbounded<PointData>();
        _cancellationTokenSource = new CancellationTokenSource();

        Task.Run(() => ProcessChannelDataAsync(_cancellationTokenSource.Token));
    }

    private async Task ProcessChannelDataAsync(CancellationToken cancellationToken)
    {
        var writeApi = _client.GetWriteApi();
        await foreach (var point in _channel.Reader.ReadAllAsync(cancellationToken))
        {
            writeApi.WritePoint(point, _bucket, _org);
            _logger.LogDebug("{point}", point.ToLineProtocol());
        }
    }

    public async Task TryCreateBucketAsync()
    {
        var organizationsApi = _client.GetOrganizationsApi();
        var organizations = await organizationsApi.FindOrganizationsAsync(org: _org);
        Organization organization;
        if (organizations is null || organizations.Count == 0)
        {
            organization = await organizationsApi.CreateOrganizationAsync(_org);
        }
        else
        {
            organization = organizations[0];
        }


        var bucketsApi = _client.GetBucketsApi();
        var bucket = await bucketsApi.FindBucketByNameAsync(_bucket);

        if (bucket is null)
        {
            // 创建时默认数据保留 30 天
            const long everySeconds = 30 * 24 * 60 * 60;
            var retentionRule =
                new BucketRetentionRules(BucketRetentionRules.TypeEnum.Expire, everySeconds);
            await bucketsApi.CreateBucketAsync(_bucket, retentionRule, organization.Id);
        }
    }

    private const string ConnectorTag = "connector";
    private const string ConnectionTag = "connection";
    private const string DataPointTag = "data_point";
    private const string DataTypeTag = "data_type";
    private const string NameTag = "variable_name";
    private const string ArchiveModeTag = "archive_mode";

    public async Task WriteDataAsync(AppVariable variable, ArchiveMode archiveMode)
    {
        var point = PointData
            .Measurement(_measurement)
            .Tag(ConnectorTag, variable.ConnectorInstance)
            .Tag(ConnectionTag, variable.ConnectionName)
            .Tag(DataPointTag, variable.DataPointName)
            .Tag(DataTypeTag, variable.DataType)
            .Tag(NameTag, variable.Name)
            .Tag(ArchiveModeTag, archiveMode.ToString())
            .Timestamp(DateTime.UtcNow, WritePrecision.S)
            .Field(variable.Name, variable.Val);

        await _channel.Writer.WriteAsync(point);
    }

    public async Task WriteDataAsync(List<AppVariable> variables, ArchiveMode archiveMode)
    {
        foreach (var variable in variables)
        {
            await WriteDataAsync(variable, archiveMode);
        }
    }

    private Task<PointData> GetDefaultPointAsync()
    {
        var point = PointData
            .Measurement(_measurement)
            .Timestamp(DateTime.UtcNow, WritePrecision.S);

        return Task.FromResult(point);
    }

    public async Task WriteDataAsync(List<(string name, string value)> tags, (string name, byte value) field)
    {
        var point = await GetDefaultPointAsync();
        foreach (var (name, value) in tags)
        {
            point = point.Tag(name, value);
        }

        point = point.Field(field.name, field.value);
        await _channel.Writer.WriteAsync(point);
    }

    public async Task WriteDataAsync(List<(string name, string value)> tags, (string name, float value) field)
    {
        var point = await GetDefaultPointAsync();
        foreach (var (name, value) in tags)
        {
            point = point.Tag(name, value);
        }

        point = point.Field(field.name, field.value);
        await _channel.Writer.WriteAsync(point);
    }

    public async Task WriteDataAsync(List<(string name, string value)> tags, (string name, double value) field)
    {
        var point = await GetDefaultPointAsync();
        foreach (var (name, value) in tags)
        {
            point = point.Tag(name, value);
        }

        point = point.Field(field.name, field.value);
        await _channel.Writer.WriteAsync(point);
    }

    public async Task WriteDataAsync(List<(string name, string value)> tags, (string name, decimal value) field)
    {
        var point = await GetDefaultPointAsync();
        foreach (var (name, value) in tags)
        {
            point = point.Tag(name, value);
        }

        point = point.Field(field.name, field.value);
        await _channel.Writer.WriteAsync(point);
    }

    public async Task WriteDataAsync(List<(string name, string value)> tags, (string name, long value) field)
    {
        var point = await GetDefaultPointAsync();
        foreach (var (name, value) in tags)
        {
            point = point.Tag(name, value);
        }

        point = point.Field(field.name, field.value);
        await _channel.Writer.WriteAsync(point);
    }

    public async Task WriteDataAsync(List<(string name, string value)> tags, (string name, ulong value) field)
    {
        var point = await GetDefaultPointAsync();
        foreach (var (name, value) in tags)
        {
            point = point.Tag(name, value);
        }

        point = point.Field(field.name, field.value);
        await _channel.Writer.WriteAsync(point);
    }

    public async Task WriteDataAsync(List<(string name, string value)> tags, (string name, uint value) field)
    {
        var point = await GetDefaultPointAsync();
        foreach (var (name, value) in tags)
        {
            point = point.Tag(name, value);
        }

        point = point.Field(field.name, field.value);
        await _channel.Writer.WriteAsync(point);
    }

    public async Task WriteDataAsync(List<(string name, string value)> tags, (string name, string value) field)
    {
        var point = await GetDefaultPointAsync();
        foreach (var (name, value) in tags)
        {
            point = point.Tag(name, value);
        }

        point = point.Field(field.name, field.value);
        await _channel.Writer.WriteAsync(point);
    }

    public async Task WriteDataAsync(List<(string name, string value)> tags, (string name, bool value) field)
    {
        var point = await GetDefaultPointAsync();
        foreach (var (name, value) in tags)
        {
            point = point.Tag(name, value);
        }

        point = point.Field(field.name, field.value);
        await _channel.Writer.WriteAsync(point);
    }


    public async Task WriteDataAsync(List<(List<(string name, string value)>, (string name, byte value))> points)
    {
        foreach (var point in points)
        {
            var p = await GetDefaultPointAsync();
            foreach (var (name, value) in point.Item1)
            {
                p = p.Tag(name, value);
            }

            p = p.Field(point.Item2.name, point.Item2.value);
            await _channel.Writer.WriteAsync(p);
        }
    }

    public async Task WriteDataAsync(List<(List<(string name, string value)>, (string name, float value))> points)
    {
        foreach (var point in points)
        {
            var p = await GetDefaultPointAsync();
            foreach (var (name, value) in point.Item1)
            {
                p = p.Tag(name, value);
            }

            p = p.Field(point.Item2.name, point.Item2.value);
            await _channel.Writer.WriteAsync(p);
        }
    }

    public async Task WriteDataAsync(List<(List<(string name, string value)>, (string name, double value))> points)
    {
        foreach (var point in points)
        {
            var p = await GetDefaultPointAsync();
            foreach (var (name, value) in point.Item1)
            {
                p = p.Tag(name, value);
            }

            p = p.Field(point.Item2.name, point.Item2.value);
            await _channel.Writer.WriteAsync(p);
        }
    }

    public async Task WriteDataAsync(List<(List<(string name, string value)>, (string name, decimal value))> points)
    {
        foreach (var point in points)
        {
            var p = await GetDefaultPointAsync();
            foreach (var (name, value) in point.Item1)
            {
                p = p.Tag(name, value);
            }

            p = p.Field(point.Item2.name, point.Item2.value);
            await _channel.Writer.WriteAsync(p);
        }
    }

    public async Task WriteDataAsync(List<(List<(string name, string value)>, (string name, long value))> points)
    {
        foreach (var point in points)
        {
            var p = await GetDefaultPointAsync();
            foreach (var (name, value) in point.Item1)
            {
                p = p.Tag(name, value);
            }

            p = p.Field(point.Item2.name, point.Item2.value);
            await _channel.Writer.WriteAsync(p);
        }
    }

    public async Task WriteDataAsync(List<(List<(string name, string value)>, (string name, ulong value))> points)
    {
        foreach (var point in points)
        {
            var p = await GetDefaultPointAsync();
            foreach (var (name, value) in point.Item1)
            {
                p = p.Tag(name, value);
            }

            p = p.Field(point.Item2.name, point.Item2.value);
            await _channel.Writer.WriteAsync(p);
        }
    }

    public async Task WriteDataAsync(List<(List<(string name, string value)>, (string name, uint value))> points)
    {
        foreach (var point in points)
        {
            var p = await GetDefaultPointAsync();
            foreach (var (name, value) in point.Item1)
            {
                p = p.Tag(name, value);
            }

            p = p.Field(point.Item2.name, point.Item2.value);
            await _channel.Writer.WriteAsync(p);
        }
    }

    public async Task WriteDataAsync(List<(List<(string name, string value)>, (string name, string value))> points)
    {
        foreach (var point in points)
        {
            var p = await GetDefaultPointAsync();
            foreach (var (name, value) in point.Item1)
            {
                p = p.Tag(name, value);
            }

            p = p.Field(point.Item2.name, point.Item2.value);
            await _channel.Writer.WriteAsync(p);
        }
    }

    public async Task WriteDataAsync(List<(List<(string name, string value)>, (string name, bool value))> points)
    {
        foreach (var point in points)
        {
            var p = await GetDefaultPointAsync();
            foreach (var (name, value) in point.Item1)
            {
                p = p.Tag(name, value);
            }

            p = p.Field(point.Item2.name, point.Item2.value);
            await _channel.Writer.WriteAsync(p);
        }
    }

    public async Task DeleteDataAsync(ArchivedVariable archivedVariable)
    {
        var deleteApi = _client.GetDeleteApi();
        var predicate = $"""
                         _measurement="{_measurement}" and
                         {ConnectorTag}="{archivedVariable.ConnectorInstance}" and
                         {ConnectionTag}="{archivedVariable.ConnectionName}" and
                         {DataPointTag}="{archivedVariable.DataPoint}" and
                         {NameTag}="{archivedVariable.Name}"
                         {ArchiveModeTag}="{archivedVariable.ArchiveMode}"
                         """;

        await deleteApi.Delete(
            DateTime.Parse("1970-01-01T00:00:00Z"),
            DateTime.UtcNow,
            predicate,
            _bucket,
            _org
        );
    }

    public async Task DeleteDataAsync(List<ArchivedVariable> archivedVariables)
    {
        foreach (var archivedVariable in archivedVariables)
        {
            await DeleteDataAsync(archivedVariable);
        }
    }

    public async Task<List<FluxTable>> QueryDataAsync(ArchivedVariable archivedVariable, string relative)
    {
        var queryApi = _client.GetQueryApi();
        var flux = $"""
                    from(bucket: "{_bucket}")
                    |> range(start: {relative})
                    |> filter(fn: (r) => r["_measurement"] == "{_measurement}")
                    |> filter(fn: (r) => r["{ConnectorTag}"] == "{archivedVariable.ConnectorInstance}")
                    |> filter(fn: (r) => r["{ConnectionTag}"] == "{archivedVariable.ConnectionName}")
                    |> filter(fn: (r) => r["{DataPointTag}"] == "{archivedVariable.DataPoint}")
                    |> filter(fn: (r) => r["{NameTag}"] == "{archivedVariable.Name}")
                    |> filter(fn: (r) => r["{ArchiveModeTag}"] == "{archivedVariable.ArchiveMode}")
                    """;
        var result = await queryApi.QueryAsync(flux, _org);
        return result;
    }

    public async Task<List<FluxTable>> QueryDataAsync(List<ArchivedVariable> archivedVariables, string relative)
    {
        var tasks = archivedVariables
            .Select(archivedVariable => QueryDataAsync(archivedVariable, relative));
        var results = await Task.WhenAll(tasks);
        return results.SelectMany(result => result).ToList();
    }

    public async Task<List<FluxTable>> QueryDataAsync(ArchivedVariable archivedVariable, string start, string stop)
    {
        var queryApi = _client.GetQueryApi();
        var flux = $"""
                    from(bucket: "{_bucket}")
                    |> range(start: {start}, stop: {stop})
                    |> filter(fn: (r) => r["_measurement"] == "{_measurement}")
                    |> filter(fn: (r) => r["{ConnectorTag}"] == "{archivedVariable.ConnectorInstance}")
                    |> filter(fn: (r) => r["{ConnectionTag}"] == "{archivedVariable.ConnectionName}")
                    |> filter(fn: (r) => r["{DataPointTag}"] == "{archivedVariable.DataPoint}")
                    |> filter(fn: (r) => r["{NameTag}"] == "{archivedVariable.Name}")
                    |> filter(fn: (r) => r["{ArchiveModeTag}"] == "{archivedVariable.ArchiveMode}")
                    """;
        var result = await queryApi.QueryAsync(flux, _org);
        return result;
    }

    public async Task<List<FluxTable>> QueryDataAsync(
        List<ArchivedVariable> archivedVariables,
        string start,
        string stop
    )
    {
        var tasks = archivedVariables
            .Select(archivedVariable => QueryDataAsync(archivedVariable, start, stop));
        var results = await Task.WhenAll(tasks);
        return results.SelectMany(result => result).ToList();
    }

    public async Task<List<FluxTable>> QueryDataAsync(List<(string name, string value)> tags, string relative)
    {
        var queryApi = _client.GetQueryApi();
        var flux = $"""
                    from(bucket: "{_bucket}")
                    |> range(start: {relative})
                    |> filter(fn: (r) => r["_measurement"] == "{_measurement}")
                    """;
        foreach (var tag in tags)
        {
            flux += $"""
                     |> filter(fn: (r) => r["{tag.name}"] == "{tag.value}")
                     """;
        }

        var result = await queryApi.QueryAsync(flux, _org);
        return result;
    }

    public async Task<List<FluxTable>> QueryDataAsync(List<(string name, string value)> tags, string start, string stop)
    {
        var queryApi = _client.GetQueryApi();
        var flux = $"""
                    from(bucket: "{_bucket}")
                    |> range(start: {start}, stop: {stop})
                    |> filter(fn: (r) => r["_measurement"] == "{_measurement}")
                    """;
        foreach (var tag in tags)
        {
            flux += $"""
                     |> filter(fn: (r) => r["{tag.name}"] == "{tag.value}")
                     """;
        }

        var result = await queryApi.QueryAsync(flux, _org);
        return result;
    }

    public async Task<List<FluxTable>> QueryDataAsync(List<List<(string name, string value)>> tags, string relative)
    {
        var tasks = tags
            .Select(tag => QueryDataAsync(tag, relative));
        var results = await Task.WhenAll(tasks);
        return results.SelectMany(result => result).ToList();
    }

    public async Task<List<FluxTable>> QueryDataAsync(List<List<(string name, string value)>> tags, string start,
        string stop)
    {
        var tasks = tags
            .Select(tag => QueryDataAsync(tag, start, stop));
        var results = await Task.WhenAll(tasks);
        return results.SelectMany(result => result).ToList();
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _client.Dispose();
    }
}
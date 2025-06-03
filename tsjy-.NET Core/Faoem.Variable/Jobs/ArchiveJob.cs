using Faoem.Variable.Definitions;
using Faoem.Variable.Models;
using Faoem.Variable.Services.InfluxDbClient;
using Faoem.Variable.Services.Variable;
using Faoem.Variable.Services.VariableArchive;
using Quartz;

namespace Faoem.Variable.Jobs;

public class ArchiveJob(
    IVariableArchiveService variableArchiveService,
    IVariableService variableService,
    IInfluxDbClientService influxDbClientService
) : IJob
{
    public static readonly JobKey JobKey = new JobKey("archive-job");

    public async Task Execute(IJobExecutionContext context)
    {
        var variables = await variableService.GetVariablesAsync();
        if (variables.Count == 0)
        {
            return;
        }

        var intervalArchivedVariables = await variableArchiveService.GetIntervalArchivedVariablesAsync();
        if (intervalArchivedVariables.Count == 0)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        List<AppVariable> archives = [];
        foreach (var intervalArchivedVariable in intervalArchivedVariables)
        {
            if (intervalArchivedVariable.ArchiveInterval == 0)
            {
                continue;
            }

            if (now % intervalArchivedVariable.ArchiveInterval != 0)
            {
                continue;
            }

            var variable = variables.FirstOrDefault(appVariable =>
                appVariable.ConnectorInstance == intervalArchivedVariable.ConnectorInstance &&
                appVariable.ConnectionName == intervalArchivedVariable.ConnectionName &&
                appVariable.DataPointName == intervalArchivedVariable.DataPoint &&
                appVariable.Name == intervalArchivedVariable.Name
            );
            if (variable is null)
            {
                continue;
            }

            archives.Add(variable);
        }

        await influxDbClientService.WriteDataAsync(archives, ArchiveMode.Period);
    }
}
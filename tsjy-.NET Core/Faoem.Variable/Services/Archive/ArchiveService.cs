using Faoem.Variable.Definitions;
using Faoem.Variable.EventArgs;
using Faoem.Variable.Models;
using Faoem.Variable.Services.InfluxDbClient;
using Faoem.Variable.Services.Variable;
using Faoem.Variable.Services.VariableArchive;
using Microsoft.Extensions.Hosting;

namespace Faoem.Variable.Services.Archive;

internal class ArchiveService : IHostedService
{
    private readonly IVariableService _variableService;
    private readonly IVariableArchiveService _variableArchiveService;
    private readonly IInfluxDbClientService _influxDbClientService;

    public ArchiveService(
        IVariableService variableService,
        IVariableArchiveService variableArchiveService,
        IInfluxDbClientService influxDbClientService
    )
    {
        _variableService = variableService;
        _variableArchiveService = variableArchiveService;
        _influxDbClientService = influxDbClientService;

        _variableService.VariableChangedAsync += VariableServiceOnVariableChangedAsync;
    }

    private async Task VariableServiceOnVariableChangedAsync(VariableChangedEventArgs arg)
    {
        var variables = arg.Variables;


        var archivedVariables = new List<AppVariable>();
        foreach (var variable in variables)
        {
            if (await _variableArchiveService.IsArchivedOnChangeAsync(variable))
            {
                archivedVariables.Add(variable);
            }
        }

        await _influxDbClientService.WriteDataAsync(archivedVariables, ArchiveMode.Change);
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
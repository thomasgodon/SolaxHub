using Microsoft.Extensions.Options;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Modbus.Models;
using SolaxHub.Solax.Services;
using System.Diagnostics;

namespace SolaxHub.Solax.Workers;

internal class SolaxModbusWorker : BackgroundService
{
    private static readonly ActivitySource ActivitySource = new(nameof(SolaxModbusWorker));
    private readonly ISolaxModbusClient _solaxModbusClient;
    private readonly ISolaxPollingService _solaxControllerService;
    private readonly SolaxModbusOptions _solaxModbusOptions;

    public SolaxModbusWorker(
        ISolaxModbusClient solaxModbusClient,
        ISolaxPollingService solaxControllerService,
        IOptions<SolaxModbusOptions> solaxModbusOptions)
    {
        _solaxModbusClient = solaxModbusClient;
        _solaxControllerService = solaxControllerService;
        _solaxModbusOptions = solaxModbusOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested is false)
        {
            using (ActivitySource.StartActivity())
            {
                await _solaxModbusClient.ConnectAsync(cancellationToken);
                await _solaxControllerService.ProcessAsync(cancellationToken);
            }

            await Task.Delay(_solaxModbusOptions.PollInterval, cancellationToken);
        }
    }
}
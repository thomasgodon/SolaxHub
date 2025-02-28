using Microsoft.Extensions.Options;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Modbus.Models;
using SolaxHub.Solax.Services;

namespace SolaxHub;

internal class SolaxModbusWorker : BackgroundService
{
    private readonly ISolaxModbusClient _solaxModbusClient;
    private readonly ISolaxControllerService _solaxControllerService;
    private readonly SolaxModbusOptions _solaxModbusOptions;

    public SolaxModbusWorker(
        ISolaxModbusClient solaxModbusClient,
        ISolaxControllerService solaxControllerService,
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
            await _solaxModbusClient.ConnectAsync(cancellationToken);
            await _solaxControllerService.ProcessAsync(cancellationToken);

            await Task.Delay(_solaxModbusOptions.PollInterval, cancellationToken);
        }
    }
}
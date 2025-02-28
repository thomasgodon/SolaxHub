using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Services;

namespace SolaxHub;

internal class Worker : BackgroundService
{
    private readonly ISolaxModbusClient _solaxModbusClient;
    private readonly ISolaxControllerService _solaxControllerService;

    public Worker(
        ISolaxModbusClient solaxModbusClient,
        ISolaxControllerService solaxControllerService)
    {
        _solaxModbusClient = solaxModbusClient;
        _solaxControllerService = solaxControllerService;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested is false)
        {
            await _solaxModbusClient.ConnectAsync(cancellationToken);
            await _solaxControllerService.ProcessAsync(cancellationToken);
        }
    }
}
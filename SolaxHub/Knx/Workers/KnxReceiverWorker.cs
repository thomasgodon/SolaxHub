using SolaxHub.Solax.Modbus.Client;

namespace SolaxHub.Knx.Workers;

internal class KnxReceiverWorker : BackgroundService
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public KnxReceiverWorker(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested is false)
        {

            await Task.Delay(2000, cancellationToken);
        }
    }
}
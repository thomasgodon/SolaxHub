using SolaxHub.Solax.Modbus.Client;
using System.Diagnostics;

namespace SolaxHub.Knx.Workers;

internal class KnxReceiverWorker : BackgroundService
{
    private static readonly ActivitySource ActivitySource = new(nameof(KnxReceiverWorker));
    private readonly ISolaxModbusClient _solaxModbusClient;

    public KnxReceiverWorker(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested is false)
        {
            using (ActivitySource.StartActivity())
            {
                await Task.Delay(2000, cancellationToken);
            }
        }
    }
}
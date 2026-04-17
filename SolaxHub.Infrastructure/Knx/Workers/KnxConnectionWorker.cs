using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SolaxHub.Infrastructure.Knx.Client;
using System.Diagnostics;

namespace SolaxHub.Infrastructure.Knx.Workers;

internal class KnxConnectionWorker : BackgroundService
{
    private static readonly ActivitySource ActivitySource = new(nameof(KnxConnectionWorker));
    private readonly IKnxClient _knxClient;

    public KnxConnectionWorker(IKnxClient knxClient)
    {
        _knxClient = knxClient;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested is false)
        {
            using (ActivitySource.StartActivity())
            {
                await _knxClient.ConnectAsync(cancellationToken);
            }
            await Task.Delay(2000, cancellationToken);
        }
    }
}

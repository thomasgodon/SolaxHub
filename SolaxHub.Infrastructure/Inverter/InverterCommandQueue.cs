using System.Threading.Channels;
using SolaxHub.Application.Inverter.Services;

namespace SolaxHub.Infrastructure;

internal sealed class InverterCommandQueue : IInverterCommandQueue
{
    private readonly Channel<Func<CancellationToken, Task>> _channel =
        Channel.CreateUnbounded<Func<CancellationToken, Task>>();

    public void Enqueue(Func<CancellationToken, Task> command)
        => _channel.Writer.TryWrite(command);

    public bool TryDequeue(out Func<CancellationToken, Task>? command)
        => _channel.Reader.TryRead(out command);
}

namespace SolaxHub.Application.Inverter.Services;

public interface IInverterCommandQueue
{
    void Enqueue(Func<CancellationToken, Task> command);
    bool TryDequeue(out Func<CancellationToken, Task>? command);
}

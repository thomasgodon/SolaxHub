using SolaxHub.Solax.Models;

namespace SolaxHub.Solax
{
    internal interface ISolaxConsumer
    {
        bool Enabled { get; }
        Task ConsumeSolaxDataAsync(SolaxData data, CancellationToken cancellation);
    }
}

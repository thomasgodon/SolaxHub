namespace SolaxHub.Solax
{
    internal interface ISolaxConsumer
    {
        bool Enabled { get; }
        Task StartAsync(CancellationToken cancellation);
    }
}

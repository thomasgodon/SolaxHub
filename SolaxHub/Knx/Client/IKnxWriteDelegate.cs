namespace SolaxHub.Knx.Client
{
    internal interface IKnxWriteDelegate
    {
        Task ProcessWriteAsync(CancellationToken cancellationToken);
    }
}

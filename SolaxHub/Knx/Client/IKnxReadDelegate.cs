namespace SolaxHub.Knx.Client
{
    internal interface IKnxReadDelegate
    {
        Task SendReadReplyAsync(CancellationToken cancellationToken);
    }
}

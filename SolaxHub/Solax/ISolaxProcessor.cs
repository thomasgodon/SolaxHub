namespace SolaxHub.Solax
{
    internal interface ISolaxProcessor
    {
        Task ProcessResult(SolaxResult result, CancellationToken cancellationToken);
    }
}

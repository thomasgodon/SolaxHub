namespace SolaxHub.Solax
{
    internal interface ISolaxWriter
    {
        void SetSolaxClient(ISolaxClient solaxClient);
        Task StartAsync(CancellationToken cancellationToken);
    }
}

namespace SolaxHub.Solax
{
    internal interface ISolaxWriter
    {
        void SetSolaxClient(ISolaxModbusClient solaxClient);
        Task StartAsync(CancellationToken cancellationToken);
    }
}

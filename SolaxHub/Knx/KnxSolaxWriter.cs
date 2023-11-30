using SolaxHub.Knx.Client;
using SolaxHub.Solax;

namespace SolaxHub.Knx
{
    internal class KnxSolaxWriter : ISolaxWriter, IKnxWriteDelegate
    {
        private readonly IKnxClient _knxClient;
        private ISolaxClient? _solaxClient;

        public KnxSolaxWriter(IKnxClient knxClient)
        {
            _knxClient = knxClient;
        }

        public void SetSolaxClient(ISolaxClient solaxClient)
        {
            _solaxClient = solaxClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _knxClient.ConnectAsync(cancellationToken);
        }

        public Task ProcessWriteAsync(CancellationToken cancellationToken)
        {
            // TODO: write to solax client
            throw new NotImplementedException();
        }
    }
}

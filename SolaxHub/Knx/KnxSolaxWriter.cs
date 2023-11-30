using Knx.Falcon;
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
            _knxClient.SetWriteDelegate(this);
        }

        public void SetSolaxClient(ISolaxClient solaxClient)
        {
            _solaxClient = solaxClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _knxClient.ConnectAsync(cancellationToken);
        }

        public Task ProcessWriteAsync(GroupAddress groupsAddress, byte[] value, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

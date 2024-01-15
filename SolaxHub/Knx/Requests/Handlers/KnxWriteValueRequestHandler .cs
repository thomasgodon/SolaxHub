using MediatR;
using SolaxHub.Solax.Modbus.Client;

namespace SolaxHub.Knx.Requests.Handlers
{
    internal class KnxWriteValueRequestHandler : IRequestHandler<KnxWriteValueRequest>
    {
        private readonly ISolaxModbusClient _solaxModbusClient;

        public KnxWriteValueRequestHandler(ISolaxModbusClient solaxModbusClient)
        {
            _solaxModbusClient = solaxModbusClient;
        }

        public Task Handle(KnxWriteValueRequest request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

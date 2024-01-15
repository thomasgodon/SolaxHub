using MediatR;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Services;

namespace SolaxHub.Knx.Requests.Handlers
{
    internal class KnxWriteValueRequestHandler : IRequestHandler<KnxWriteValueRequest>
    {
        private readonly ISolaxControllerService _solaxControllerService;

        public KnxWriteValueRequestHandler(ISolaxControllerService solaxControllerService)
        {
            _solaxControllerService = solaxControllerService;
        }

        public Task Handle(KnxWriteValueRequest request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

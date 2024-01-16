using MediatR;

namespace SolaxHub.Solax.Requests.Handlers
{
    internal class CalculatePowerControlRequestHandler : IRequestHandler<CalculatePowerControlRequest, byte[]?>
    {
        public CalculatePowerControlRequestHandler()
        {
        }

        public Task<byte[]?> Handle(CalculatePowerControlRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

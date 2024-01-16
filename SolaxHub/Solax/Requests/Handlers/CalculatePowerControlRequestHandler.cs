using MediatR;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Requests.Handlers
{
    internal class CalculatePowerControlRequestHandler : IRequestHandler<CalculatePowerControlRequest, SolaxPowerControlCalculation>
    {
        public CalculatePowerControlRequestHandler()
        {
        }

        public Task<SolaxPowerControlCalculation> Handle(CalculatePowerControlRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

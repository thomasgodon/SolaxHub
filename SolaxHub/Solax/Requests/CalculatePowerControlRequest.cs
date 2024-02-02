using MediatR;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Requests
{
    internal class CalculatePowerControlRequest : IRequest<SolaxPowerControlCalculation>
    {
        public CalculatePowerControlRequest(SolaxData solaxData)
        {
            SolaxData = solaxData;
        }

        public SolaxData SolaxData { get; init; }
    }
}

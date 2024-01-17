using MediatR;
using SolaxHub.Solax.Models;
using SolaxHub.Solax.Services;

namespace SolaxHub.Solax.Requests.Handlers
{
    internal class CalculatePowerControlRequestHandler : IRequestHandler<CalculatePowerControlRequest, SolaxPowerControlCalculation>
    {
        private readonly ISolaxControllerService _solaxControllerService;

        public CalculatePowerControlRequestHandler(ISolaxControllerService solaxControllerService)
        {
            _solaxControllerService = solaxControllerService;
        }

        public Task<SolaxPowerControlCalculation> Handle(CalculatePowerControlRequest request, CancellationToken cancellationToken)
        {
            var result = new SolaxPowerControlCalculation(
                modbusPowerControl: _solaxControllerService.PowerControlMode != SolaxPowerControlMode.Disabled,
                remoteControlActivePower: 0,
                remoteControlReactivePower: 0);

            return Task.FromResult(result);
        }
    }
}

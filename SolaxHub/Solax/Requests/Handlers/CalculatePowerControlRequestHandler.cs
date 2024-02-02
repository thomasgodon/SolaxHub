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
                modbusPowerControl: PowerControlModeEnabled(),
                remoteControlActivePower: PowerControlModeEnabled()
                    ? CalculateActivePower(_solaxControllerService.PowerControlMode, request.SolaxData)
                    : 0,
                remoteControlReactivePower: 0);

            return Task.FromResult(result);
        }

        private bool PowerControlModeEnabled()
            => _solaxControllerService.PowerControlMode != SolaxPowerControlMode.Disabled;

        private static double CalculateActivePower(SolaxPowerControlMode mode, SolaxData data)
            => mode switch
            {
                SolaxPowerControlMode.Disabled => 0,
                SolaxPowerControlMode.EnabledPowerControl => 1500,
                SolaxPowerControlMode.EnabledGridControl => 0,
                SolaxPowerControlMode.EnabledBatteryControl => 0,
                SolaxPowerControlMode.EnabledFeedInPriority => 0,
                SolaxPowerControlMode.EnabledNoDischarge => 0,
                SolaxPowerControlMode.EnabledQuantityControl => 0,
                SolaxPowerControlMode.EnabledSocTargetControl => 0,
                _ => 0,
            };
    }
}

using MediatR;
using SolaxHub.Solax.Models;
using SolaxHub.Solax.Services;

namespace SolaxHub.Solax.Requests.Handlers
{
    internal class CalculatePowerControlRequestHandler : IRequestHandler<CalculatePowerControlRequest, SolaxPowerControlCalculation>
    {
        private readonly ILogger<CalculatePowerControlRequestHandler> _logger;
        private readonly ISolaxControllerService _solaxControllerService;

        public CalculatePowerControlRequestHandler(
            ILogger<CalculatePowerControlRequestHandler> logger,
            ISolaxControllerService solaxControllerService
            )
        {
            _logger = logger;
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

            _logger.LogInformation("Enabled: {enabled}, active power: {activePower}", result.ModbusPowerControl, result.RemoteControlActivePower);

            return Task.FromResult(result);
        }

        private bool PowerControlModeEnabled()
            => _solaxControllerService.PowerControlMode != SolaxPowerControlMode.Disabled;

        private static double CalculateActivePower(SolaxPowerControlMode mode, SolaxData data)
            => mode switch
            {
                SolaxPowerControlMode.Disabled => 0,
                SolaxPowerControlMode.EnabledPowerControl => 0,
                SolaxPowerControlMode.EnabledGridControl => 2000,
                SolaxPowerControlMode.EnabledBatteryControl => 0,
                SolaxPowerControlMode.EnabledFeedInPriority => 0,
                SolaxPowerControlMode.EnabledNoDischarge => 0,
                SolaxPowerControlMode.EnabledQuantityControl => 0,
                SolaxPowerControlMode.EnabledSocTargetControl => 0,
                _ => 0,
            };
    }
}

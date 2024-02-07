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
            var (enabled, activePower, reactivePower) = CalculateValues(_solaxControllerService.PowerControlMode, request.SolaxData);
            var result = new SolaxPowerControlCalculation(
                enabled,
                activePower,
                reactivePower);

            return Task.FromResult(result);
        }

        private (bool Enabled, double ActivePower, double ReactivePower) CalculateValues(SolaxPowerControlMode mode, SolaxData data)
            => mode switch
            {
                // power control disabled
                SolaxPowerControlMode.Disabled => (false, 0, 0),

                // battery will be charged from the grid, grid import will be limited to import limit value
                SolaxPowerControlMode.EnabledGridControl => (true, CalculateGridControlActivePower(data), 0),

                // battery will be charged from the grid
                SolaxPowerControlMode.EnabledBatteryControl => (true, _solaxControllerService.PowerControlBatteryChargeLimit, 0),
                SolaxPowerControlMode.EnabledNoDischarge => (true, 0, 0),
                _ => (false, 0, 0)
            };

        private double CalculateGridControlActivePower(SolaxData data)
            => _solaxControllerService.PowerControlImportLimit - (data.HouseLoad - data.PvCurrent1) > 0
                ? _solaxControllerService.PowerControlImportLimit - (data.HouseLoad - data.PvCurrent1)
                : 0;
    }
}

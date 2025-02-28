using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Services;

public interface ISolaxControllerService
{
    SolaxPowerControlMode PowerControlMode { get; set; }
    double PowerControlImportLimit { get; set; }
    double PowerControlBatteryChargeLimit { get; set; }
    double BatteryDischargeLimit { get; set; }
    SolaxInverterUseMode InverterUseMode { get; set; }

    Task ProcessAsync(CancellationToken cancellationToken);
}
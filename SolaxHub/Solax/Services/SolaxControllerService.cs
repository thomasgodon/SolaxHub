using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Services;

internal class SolaxControllerService : ISolaxControllerService
{
    public SolaxPowerControlMode PowerControlMode { get; set; } = SolaxPowerControlMode.Disabled;
    public double PowerControlImportLimit { get; set; }
    public double PowerControlBatteryChargeLimit { get; set; }
    public SolaxInverterUseMode InverterUseMode { get; set; }
}

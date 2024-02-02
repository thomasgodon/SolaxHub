using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Services
{
    internal interface ISolaxControllerService
    {
        SolaxPowerControlMode PowerControlMode { get; }
        Task SetRemoteControlPowerControlModeAsync(SolaxPowerControlMode powerControlMode);
    }
}

using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Services
{
    internal interface ISolaxControllerService
    {
        SolaxRemoteControlPowerControlMode PowerControlMode { get; }
        Task SetRemoteControlPowerControlModeAsync(SolaxRemoteControlPowerControlMode powerControlMode);
    }
}

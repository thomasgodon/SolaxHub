using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Services
{
    internal class SolaxControllerService : ISolaxControllerService
    {
        public SolaxControllerService()
        {
        }

        public SolaxRemoteControlPowerControlMode PowerControlMode { get; private set; } = SolaxRemoteControlPowerControlMode.Disabled;

        public Task SetRemoteControlPowerControlModeAsync(SolaxRemoteControlPowerControlMode powerControlMode)
        {
            PowerControlMode = powerControlMode;
            return Task.CompletedTask;
        }
    }
}

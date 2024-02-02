using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Services
{
    internal class SolaxControllerService : ISolaxControllerService
    {
        public SolaxControllerService()
        {
        }

        public SolaxPowerControlMode PowerControlMode { get; private set; } = SolaxPowerControlMode.Disabled;

        public Task SetRemoteControlPowerControlModeAsync(SolaxPowerControlMode powerControlMode)
        {
            PowerControlMode = powerControlMode;
            return Task.CompletedTask;
        }
    }
}

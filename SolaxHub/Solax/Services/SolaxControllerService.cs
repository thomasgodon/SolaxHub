using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Services
{
    internal class SolaxControllerService : ISolaxControllerService
    {
        public SolaxControllerService()
        {
        }

        public Task SetRemoteControlPowerControlModeAsync(SolaxRemoteControlPowerControlMode powerControlMode)
        {
            return Task.CompletedTask;
        }
    }
}

using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Services
{
    internal class SolaxControllerService : ISolaxControllerService
    {
        private readonly ISolaxModbusClient _solaxModbusClient;

        public SolaxControllerService(ISolaxModbusClient solaxModbusClient)
        {
            _solaxModbusClient = solaxModbusClient;
        }

        public async Task SetInverterUseModeAsync(SolaxInverterUseMode useMode, CancellationToken cancellationToken)
        {
            await _solaxModbusClient.SetSolarChargerUseModeAsync(useMode, cancellationToken);
        }
    }
}

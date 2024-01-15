using SolaxHub.Solax.Modbus.Client;

namespace SolaxHub.Solax.Services
{
    internal class SolaxControllerService : ISolaxControllerService
    {
        private readonly ISolaxModbusClient _solaxModbusClient;

        public SolaxControllerService(ISolaxModbusClient solaxModbusClient)
        {
            _solaxModbusClient = solaxModbusClient;
        }
    }
}

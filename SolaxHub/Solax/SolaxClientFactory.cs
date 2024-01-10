using SolaxHub.Solax.Modbus;

namespace SolaxHub.Solax
{
    internal class SolaxClientFactory : ISolaxClientFactory
    {
        private readonly SolaxModbusClient _modbusClient;

        public SolaxClientFactory(SolaxModbusClient modbusClient)
        {
            _modbusClient = modbusClient;
        }

        public ISolaxModbusClient CreateSolaxClient() => _modbusClient;
    }
}

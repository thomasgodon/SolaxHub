using Microsoft.Extensions.Options;
using SolaxHub.Solax.Http;
using SolaxHub.Solax.Modbus;

namespace SolaxHub.Solax
{
    internal class SolaxClientFactory : ISolaxClientFactory
    {
        private readonly SolaxHttpOptions _httpOptions;
        private readonly SolaxModbusOptions _modbusOptions;
        private readonly SolaxHttpClient _httpClient;
        private readonly SolaxModbusClient _modbusClient;

        public SolaxClientFactory(SolaxHttpClient httpClient, SolaxModbusClient modbusClient, IOptions<SolaxHttpOptions> httpOptions, IOptions<SolaxModbusOptions> modbusOptions)
        {
            _httpOptions = httpOptions.Value;
            _modbusOptions = modbusOptions.Value;
            _httpClient = httpClient;
            _modbusClient = modbusClient;
        }
        public ISolaxClient? CreateSolaxClient()
        {
            if (_modbusOptions.Enabled)
            {
                return _modbusClient;
            }

            return _httpOptions.Enabled 
                ? _httpClient 
                : null;
        }
    }
}

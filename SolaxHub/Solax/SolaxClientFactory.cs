using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (_httpOptions.Enabled)
            {
                return _httpClient;
            }

            return null;
        }
    }
}

using System.Net;
using FluentModbus;
using Microsoft.Extensions.Options;

namespace SolaxHub.Solax.Modbus
{
    internal class SolaxModbusClient : ISolaxClient
    {
        private readonly ILogger<SolaxModbusClient> _logger;
        private readonly SolaxModbusOptions _solaxModbusOptions;
        private readonly ISolaxProcessorService _solaxProcessorService;
        private readonly ModbusTcpClient _modbusClient;

        public SolaxModbusClient(ILogger<SolaxModbusClient> logger, ISolaxProcessorService solaxProcessorService, IOptions<SolaxModbusOptions> solaxModbusOptions)
        {
            _logger = logger;
            _solaxModbusOptions = solaxModbusOptions.Value;
            _solaxProcessorService = solaxProcessorService;
            _modbusClient = new ModbusTcpClient();
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            var endPoint = await GetEndPointAsync(cancellationToken);
            _modbusClient.Connect(endPoint);

            await Task.Run(async () =>
            {
                // Keep this task alive until it is cancelled
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (_modbusClient.IsConnected is false)
                    {
                        continue;
                    }

                    await Task.Delay(_solaxModbusOptions.PollInterval, cancellationToken);
                    var data = await GetRealTimeData(cancellationToken);
                    await _solaxProcessorService.ProcessData(data.ToSolaxData(), cancellationToken);
                }
            }, cancellationToken);
        }

        private async Task<SolaxModbusData> GetRealTimeData(CancellationToken cancellationToken)
        {
            return new SolaxModbusData();
        }

        private async Task<IPEndPoint> GetEndPointAsync(CancellationToken cancellationToken)
        {
            var hostEntry = await Dns.GetHostEntryAsync(_solaxModbusOptions.Host, cancellationToken);

            if (hostEntry.AddressList.Length == 0)
            {
                throw new ArgumentOutOfRangeException($"Could not resolve ip for host '{_solaxModbusOptions.Host}'");
            }

            return new IPEndPoint(hostEntry.AddressList[0].MapToIPv4(), _solaxModbusOptions.Port);
        }
    }
}

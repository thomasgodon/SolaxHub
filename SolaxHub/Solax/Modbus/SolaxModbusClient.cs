using System.Net;
using FluentModbus;
using Microsoft.Extensions.Options;

namespace SolaxHub.Solax.Modbus
{
    internal partial class SolaxModbusClient : ISolaxClient
    {
        private readonly SolaxModbusOptions _solaxModbusOptions;
        private readonly ISolaxProcessorService _solaxProcessorService;
        private readonly IEnumerable<ISolaxWriter> _solaxWriters;
        private readonly ModbusTcpClient _modbusClient;
        private readonly ILogger<SolaxModbusClient> _logger;
        private const byte UnitIdentifier = 0x00; // 0x00 and 0xFF are the defaults for TCP/IP-only Modbus devices.

        public SolaxModbusClient(ILogger<SolaxModbusClient> logger, ISolaxProcessorService solaxProcessorService, IEnumerable<ISolaxWriter> solaxWriters, IOptions<SolaxModbusOptions> solaxModbusOptions)
        {
            _solaxModbusOptions = solaxModbusOptions.Value;
            _solaxProcessorService = solaxProcessorService;
            _solaxWriters = solaxWriters;
            _logger = logger;
            _modbusClient = new ModbusTcpClient();
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            var endPoint = await GetEndPointAsync(cancellationToken);
            _modbusClient.ReadTimeout = 1000;

            await Task.Run(async () =>
            {
                // Keep this task alive until it is cancelled
                while (cancellationToken.IsCancellationRequested is false)
                {
                    if (_modbusClient.IsConnected is false)
                    {
                        _modbusClient.Connect(endPoint, ModbusEndianness.BigEndian);

                        // set solax client instance to all writers & start writer
                        foreach (var solaxWriter in _solaxWriters)
                        {
                            solaxWriter.SetSolaxClient(this);
                            await solaxWriter.StartAsync(cancellationToken);
                        }
                        continue;
                    }

                    await Task.Delay(_solaxModbusOptions.PollInterval, cancellationToken);

                    try
                    {
                        var data = await GetSolaxModbusData(cancellationToken);
                        await _solaxProcessorService.ProcessData(data.ToSolaxData(), cancellationToken);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "{message}", e.Message);
                    }
                }
            }, cancellationToken);
        }

        public async Task WriteRegisterAsync(byte identifier, ushort registerAddress, byte[] value, CancellationToken cancellationToken)
        {
            if (_modbusClient.IsConnected is false)
            {
                _logger.LogError("Modbus client not connected");
                return;
            }

            await _modbusClient.WriteSingleRegisterAsync(identifier, registerAddress, value, cancellationToken);
        }

        private async Task<IPEndPoint> GetEndPointAsync(CancellationToken cancellationToken)
        {
            if (IPAddress.TryParse(_solaxModbusOptions.Host, out var parsedIp))
            {
                return new IPEndPoint(parsedIp, _solaxModbusOptions.Port);
            }

            var hostEntry = await Dns.GetHostEntryAsync(_solaxModbusOptions.Host, cancellationToken);

            if (hostEntry.AddressList.Length == 0)
            {
                throw new ArgumentOutOfRangeException($"Could not resolve ip for host '{_solaxModbusOptions.Host}'");
            }

            parsedIp = hostEntry.AddressList[0].MapToIPv4();
            return new IPEndPoint(parsedIp, _solaxModbusOptions.Port);
        }
    }
}

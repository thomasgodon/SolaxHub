using System.Net;
using System.Text.Json;
using FluentModbus;
using MediatR;
using Microsoft.Extensions.Options;
using SolaxHub.Solax.Modbus.Models;
using SolaxHub.Solax.Models;
using SolaxHub.Solax.Notifications;

namespace SolaxHub.Solax.Modbus.Client
{
    internal partial class SolaxModbusClient : ISolaxModbusClient
    {
        private readonly SolaxModbusOptions _solaxModbusOptions;
        private readonly ModbusTcpClient _modbusClient;
        private readonly ILogger<SolaxModbusClient> _logger;
        private readonly IPublisher _publisher;
        private const byte UnitIdentifier = 0x00;

        public SolaxModbusClient(
            ILogger<SolaxModbusClient> logger, 
            IOptions<SolaxModbusOptions> solaxModbusOptions,
            IPublisher publisher)
        {
            _solaxModbusOptions = solaxModbusOptions.Value;
            _logger = logger;
            _publisher = publisher;
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

                        // unlock advanced inverter
                        await SetLockStateAsync(SolaxLockState.UnlockedAdvanced, cancellationToken);

                        continue;
                    }

                    await Task.Delay(_solaxModbusOptions.PollInterval, cancellationToken);

                    try
                    {
                        _lastReceivedData = await GetSolaxModbusData(cancellationToken);
                        _logger.LogTrace("{message}", JsonSerializer.Serialize(_lastReceivedData));
                        await _publisher.Publish(new SolaxDataArrivedNotification(_lastReceivedData), cancellationToken);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "{message}", e.Message);
                    }
                }
            }, cancellationToken);
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

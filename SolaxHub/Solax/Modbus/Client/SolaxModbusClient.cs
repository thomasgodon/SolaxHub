using System.Net;
using System.Text.Json;
using FluentModbus;
using MediatR;
using Microsoft.Extensions.Options;
using SolaxHub.Solax.Extensions;
using SolaxHub.Solax.Modbus.Models;
using SolaxHub.Solax.Models;
using SolaxHub.Solax.Notifications;
using SolaxHub.Solax.Requests;

namespace SolaxHub.Solax.Modbus.Client
{
    internal partial class SolaxModbusClient : ISolaxModbusClient
    {
        private readonly SolaxModbusOptions _solaxModbusOptions;
        private readonly ModbusTcpClient _modbusClient;
        private readonly ILogger<SolaxModbusClient> _logger;
        private readonly IPublisher _publisher;
        private readonly ISender _sender;
        private const byte UnitIdentifier = 0x00;

        public SolaxModbusClient(
            ILogger<SolaxModbusClient> logger, 
            IOptions<SolaxModbusOptions> solaxModbusOptions,
            IPublisher publisher,
            ISender sender)
        {
            _solaxModbusOptions = solaxModbusOptions.Value;
            _logger = logger;
            _publisher = publisher;
            _sender = sender;
            _modbusClient = new ModbusTcpClient();
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            var endPoint = await GetEndPointAsync(cancellationToken);
            _modbusClient.ReadTimeout = 1000;

            await Task.Run(async () =>
            {
                // Keep this task alive until it is cancelled
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (!_modbusClient.IsConnected)
                    {
                        _modbusClient.Connect(endPoint, ModbusEndianness.BigEndian);

                        if (_modbusClient.IsConnected)
                        {
                            _logger.LogInformation("Connected to {Host} at port: {Port}", endPoint.Address, endPoint.Port);
                        }
                        else
                        {
                            _logger.LogError("Something went wrong when trying to connect to {Host} at port: {Port}", endPoint.Address, endPoint.Port);
                        }

                        // unlock advanced inverter
                        var lockState = (await GetLockStateAsync(cancellationToken)).ToSolaxLockState();
                        if (lockState != SolaxLockState.UnlockedAdvanced)
                        {
                            _logger.LogWarning("Current lock state: '{CurrentState}. Unlocking...'", lockState);
                            await SetLockStateAsync(SolaxLockState.UnlockedAdvanced, cancellationToken);
                        }

                        _logger.LogInformation("Lock state: {LockState}", SolaxLockState.UnlockedAdvanced);

                        continue;
                    }

                    await Task.Delay(_solaxModbusOptions.PollInterval, cancellationToken);

                    try
                    {
                        // get latest read values
                        _lastReceivedData = await GetSolaxModbusData(cancellationToken);
                        _logger.LogTrace("{Message}", JsonSerializer.Serialize(_lastReceivedData));

                        // calculate & set power control mode
                        var powerControlCalculation = await _sender.Send(new CalculateRemoteControlRequest(_lastReceivedData), cancellationToken);
                        await SetPowerControlAsync(powerControlCalculation.Mode, powerControlCalculation.Data, cancellationToken);

                        // set charger use mode
                        var chargerUseMode = await _sender.Send(new GetChargerUseModeRequest(), cancellationToken);
                        if (_lastReceivedData.InverterUseMode != chargerUseMode && chargerUseMode != SolaxInverterUseMode.Unknown)
                        {
                            // we only want to update if the use mode has changed!
                            //await SetSolarChargerUseModeAsync(chargerUseMode, cancellationToken);
                        }

                        // notify new solax data has arrived
                        await _publisher.Publish(new SolaxDataArrivedNotification(_lastReceivedData), cancellationToken);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "{Message}", e.Message);
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

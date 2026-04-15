using FluentModbus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SolaxHub.Infrastructure.Modbus.Options;
using System.Diagnostics;
using System.Net;

namespace SolaxHub.Infrastructure.Modbus.Client;

internal class SolaxModbusClient : ISolaxModbusClient
{
    private static readonly ActivitySource ActivitySource = new(nameof(SolaxModbusClient));
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ModbusOptions _options;
    private readonly ModbusTcpClient _modbusClient;
    private readonly ILogger<SolaxModbusClient> _logger;

    public bool IsConnected => _modbusClient.IsConnected;

    public SolaxModbusClient(ILogger<SolaxModbusClient> logger, IOptions<ModbusOptions> options)
    {
        _options = options.Value;
        _logger = logger;
        _modbusClient = new ModbusTcpClient { ReadTimeout = 10000 };
    }

    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        using (ActivitySource.StartActivity())
        {
            IPEndPoint endPoint = await GetEndPointAsync(cancellationToken);

            if (_modbusClient.IsConnected)
            {
                _logger.LogTrace("Still connected to {Host} at port: {Port}", endPoint.Address, endPoint.Port);
                return;
            }

            _modbusClient.Connect(endPoint, ModbusEndianness.BigEndian);

            if (_modbusClient.IsConnected)
                _logger.LogInformation("Connected to {Host} at port: {Port}", endPoint.Address, endPoint.Port);
            else
                _logger.LogError("Something went wrong when trying to connect to {Host} at port: {Port}", endPoint.Address, endPoint.Port);
        }
    }

    public async Task<Memory<byte>> ReadHoldingRegistersAsync(ushort startingAddress, ushort quantity, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try { return await _modbusClient.ReadHoldingRegistersAsync(_options.UnitIdentifier, startingAddress, quantity, cancellationToken); }
        finally { _semaphore.Release(); }
    }

    public async Task<Memory<byte>> ReadInputRegistersAsync(ushort startingAddress, ushort quantity, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try { return await _modbusClient.ReadInputRegistersAsync(_options.UnitIdentifier, startingAddress, quantity, cancellationToken); }
        finally { _semaphore.Release(); }
    }

    public async Task WriteSingleRegisterAsync(ushort startingAddress, byte[] value, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try { await _modbusClient.WriteSingleRegisterAsync(_options.UnitIdentifier, startingAddress, BitConverter.ToInt16(value), cancellationToken); }
        finally { _semaphore.Release(); }
    }

    public async Task WriteMultipleRegistersAsync(ushort startingAddress, byte[] value, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try { await _modbusClient.WriteMultipleRegistersAsync(_options.UnitIdentifier, startingAddress, value, cancellationToken); }
        finally { _semaphore.Release(); }
    }

    private async Task<IPEndPoint> GetEndPointAsync(CancellationToken cancellationToken)
    {
        if (IPAddress.TryParse(_options.Host, out IPAddress? parsedIp))
            return new IPEndPoint(parsedIp, _options.Port);

        IPHostEntry hostEntry = await Dns.GetHostEntryAsync(_options.Host, cancellationToken);

        if (hostEntry.AddressList.Length == 0)
            throw new ArgumentOutOfRangeException($"Could not resolve ip for host '{_options.Host}'");

        parsedIp = hostEntry.AddressList[0].MapToIPv4();
        return new IPEndPoint(parsedIp, _options.Port);
    }
}

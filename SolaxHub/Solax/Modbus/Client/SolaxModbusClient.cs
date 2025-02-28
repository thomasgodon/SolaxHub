using System.Net;
using FluentModbus;
using Microsoft.Extensions.Options;
using SolaxHub.Solax.Modbus.Models;

namespace SolaxHub.Solax.Modbus.Client;

internal partial class SolaxModbusClient : ISolaxModbusClient
{
    private readonly SolaxModbusOptions _solaxModbusOptions;
    private readonly ModbusTcpClient _modbusClient;
    private readonly ILogger<SolaxModbusClient> _logger;

    public bool IsConnected => _modbusClient.IsConnected;

    public SolaxModbusClient(
        ILogger<SolaxModbusClient> logger, 
        IOptions<SolaxModbusOptions> solaxModbusOptions)
    {
        _solaxModbusOptions = solaxModbusOptions.Value;
        _logger = logger;
        _modbusClient = new ModbusTcpClient();

        _modbusClient.ReadTimeout = 1000;
    }

    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        IPEndPoint endPoint = await GetEndPointAsync(cancellationToken);

        if (_modbusClient.IsConnected)
        {
            _logger.LogDebug("Still connected to {Host} at port: {Port}", endPoint.Address, endPoint.Port);
            return;
        }

        _modbusClient.Connect(endPoint, ModbusEndianness.BigEndian);

        if (_modbusClient.IsConnected)
        {
            _logger.LogInformation("Connected to {Host} at port: {Port}", endPoint.Address, endPoint.Port);
        }
        else
        {
            _logger.LogError("Something went wrong when trying to connect to {Host} at port: {Port}", endPoint.Address, endPoint.Port);
        }
    }

    public async Task<Memory<byte>> ReadHoldingRegistersAsync(byte unitIdentifier, ushort startingAddress, ushort quantity, CancellationToken cancellationToken)
        => await _modbusClient.ReadHoldingRegistersAsync(unitIdentifier, startingAddress, quantity, cancellationToken);

    public async Task<Memory<byte>> ReadInputRegistersAsync(byte unitIdentifier, ushort startingAddress, ushort quantity, CancellationToken cancellationToken)
        => await _modbusClient.ReadInputRegistersAsync(unitIdentifier, startingAddress, quantity, cancellationToken);

    public async Task WriteSingleRegisterAsync(int unitIdentifier, int registerAddress, ushort value, CancellationToken cancellationToken)
        => await _modbusClient.WriteSingleRegisterAsync(unitIdentifier, registerAddress, value, cancellationToken);

    public async Task WriteMultipleRegistersAsync(byte unitIdentifier, ushort startingAddress, byte[] dataset, CancellationToken cancellationToken)
        => await _modbusClient.WriteMultipleRegistersAsync(unitIdentifier, startingAddress, dataset, cancellationToken);

    private async Task<IPEndPoint> GetEndPointAsync(CancellationToken cancellationToken)
    {
        if (IPAddress.TryParse(_solaxModbusOptions.Host, out IPAddress? parsedIp))
        {
            return new IPEndPoint(parsedIp, _solaxModbusOptions.Port);
        }

        IPHostEntry hostEntry = await Dns.GetHostEntryAsync(_solaxModbusOptions.Host, cancellationToken);

        if (hostEntry.AddressList.Length == 0)
        {
            throw new ArgumentOutOfRangeException($"Could not resolve ip for host '{_solaxModbusOptions.Host}'");
        }

        parsedIp = hostEntry.AddressList[0].MapToIPv4();
        return new IPEndPoint(parsedIp, _solaxModbusOptions.Port);
    }
}
using MediatR;
using SolaxHub.Solax.Extensions;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Models;
using System.Buffers.Binary;
using System.Text;

namespace SolaxHub.Solax.Services;

internal class SolaxControllerService : ISolaxControllerService
{
    private readonly ILogger<SolaxControllerService> _logger;
    private readonly ISolaxModbusClient _solaxModbusClient;
    private readonly IPublisher _publisher;

    private const byte UnitIdentifier = 0x00;

    public SolaxPowerControlMode PowerControlMode { get; set; } = SolaxPowerControlMode.Disabled;
    public double PowerControlImportLimit { get; set; }
    public double PowerControlBatteryChargeLimit { get; set; }
    public double BatteryDischargeLimit { get; set; }
    public SolaxInverterUseMode InverterUseMode { get; set; }

    public SolaxControllerService(
        ILogger<SolaxControllerService> logger,
        ISolaxModbusClient solaxModbusClient,
        IPublisher publisher)
    {
        _logger = logger;
        _solaxModbusClient = solaxModbusClient;
        _publisher = publisher;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        if (_solaxModbusClient.IsConnected is false)
        {
            return;
        }

        await UnlockInverterAsync(cancellationToken);

        // read registers


    }

    #region Write Methods
    private async Task SetSolarChargerUseModeAsync(SolaxInverterUseMode useMode, CancellationToken cancellationToken)
    {
        const ushort registerAddress = 0x001F;
        await _solaxModbusClient.WriteSingleRegisterAsync(UnitIdentifier, registerAddress, (ushort)useMode, cancellationToken);
    }

    private async Task SetLockStateAsync(SolaxLockState lockState, CancellationToken cancellationToken)
    {
        
        await _solaxModbusClient.WriteSingleRegisterAsync(UnitIdentifier, registerAddress, BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((ushort)lockState)), cancellationToken);
    }

    private async Task SetPowerControlAsync(SolaxPowerControlMode mode, byte[] data, CancellationToken cancellationToken)
    {
        if (!ShouldSendPowerControl())
        {
            return;
        }

        const ushort registerAddress = 0x7C;
        
        _logger.LogTrace(BitConverter.ToString(data));
        await _solaxModbusClient.WriteMultipleRegistersAsync(UnitIdentifier, registerAddress, data, cancellationToken);
    }

    private async Task SetBatteryDischargeMaxCurrent(double maxCurrent, CancellationToken cancellationToken)
    {
        const ushort registerAddress = 0x0025;
        
        // Scale the double value to fit into an uint16 range
        // Adjust the scale factor depending on the expected range
        ushort scaledValue = (ushort)Math.Clamp(maxCurrent, 0, ushort.MaxValue); // Clamps between 0 and 65535

        await _solaxModbusClient.WriteSingleRegisterAsync(UnitIdentifier, registerAddress, scaledValue, cancellationToken);
    }

    private bool ShouldSendPowerControl()
    {
        if (!_powerControlWatch.IsRunning)
        {
            _powerControlWatch.Start();
            return true;
        }

        if (_powerControlWatch.Elapsed < PowerControlInterval)
        {
            return false;
        }

        _powerControlWatch.Restart();
        return true;
    }

    private async Task UnlockInverterAsync(CancellationToken cancellationToken)
    {
        SolaxLockState lockState = (await GetLockStateAsync(cancellationToken)).ToSolaxLockState();
        if (lockState != SolaxLockState.UnlockedAdvanced)
        {
            _logger.LogWarning("Current lock state: '{CurrentState}. Unlocking...'", lockState);
            await SetLockStateAsync(SolaxLockState.UnlockedAdvanced, cancellationToken);
        }

        _logger.LogInformation("Lock state: {LockState}", SolaxLockState.UnlockedAdvanced);
    }
    #endregion

    #region Read Methods

    private async Task<string> GetRegistrationCodePocketAsync(CancellationToken cancellationToken)
    {
        const ushort startingAddress = 170;
        const ushort count = 5;
        var data = await _solaxModbusClient.ReadHoldingRegistersAsync(UnitIdentifier, startingAddress, count, cancellationToken);
        return Encoding.ASCII.GetString(data.ToArray());
    }

    private async Task<ushort> GetInverterVoltageAsync(CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }

    private async Task<short> GetInverterPowerAsync(CancellationToken cancellationToken)
    {
        const ushort startingAddress = 2;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<short>(UnitIdentifier, startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }

    private async Task<ushort> GetBatteryCapacityAsync(CancellationToken cancellationToken)
    {
        const ushort startingAddress = 28;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }

    private async Task<short> GetBatteryPowerAsync(CancellationToken cancellationToken)
    {
        const ushort startingAddress = 22;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<short>(UnitIdentifier, startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }

    private async Task<ushort> GetPvVolt1Async(CancellationToken cancellationToken)
    {
        const ushort startingAddress = 3;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }

    private async Task<ushort> GetPvCurrent1Async(CancellationToken cancellationToken)
    {
        const ushort startingAddress = 6;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }

    private async Task<ushort> GetPvPower1RAsync(CancellationToken cancellationToken)
    {
        const ushort startingAddress = 10;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }

    private async Task<double> GetSolarEnergyTodayAsync(CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0x96;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return Math.Round(data.ToArray()[0] * 0.1, 2);
    }

    private async Task<double> GetSolarEnergyTotalAsync(CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0x94;
        const ushort count = 2;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return Math.Round((data.ToArray()[1] << 16 | data.ToArray()[0] & 0xffff) * 0.1, 2);
    }

    private async Task<ushort> GetInverterStatusAsync(CancellationToken cancellationToken)
    {
        const ushort startingAddress = 9;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }

    private async Task<int> GetFeedInPowerAsync(CancellationToken cancellationToken)
    {
        const ushort startingAddress = 70;
        const ushort count = 2;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return data.ToArray()[1] << 16 | data.ToArray()[0] & 0xffff;
    }

    private async Task<double> GetFeedInEnergyAsync(CancellationToken cancellationToken)
    {
        const ushort startingAddress = 72;
        const ushort count = 2;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return Math.Round((data.ToArray()[1] << 16 | data.ToArray()[0] & 0xffff) * 0.01, 2);
    }

    private async Task<double> GetConsumeEnergyAsync(CancellationToken cancellationToken)
    {
        const ushort startingAddress = 74;
        const ushort count = 2;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return Math.Round((data.ToArray()[1] << 16 | data.ToArray()[0] & 0xffff) * 0.01, 2);
    }

    private async Task<ushort> GetSolarChargerUseModeAsync(CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0x008B;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadHoldingRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }

    private async Task<double> GetTodayBatteryOutputEnergyAsync(CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0x0020;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return Math.Round(data.ToArray()[0] * 0.1, 2);
    }

    private async Task<double> GetTotalBatteryOutputEnergyAsync(CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0x001D;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return Math.Round(data.ToArray()[0] * 0.1, 2);
    }

    private async Task<double> GetTodayBatteryInputEnergyAsync(CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0x0023;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return Math.Round(data.ToArray()[0] * 0.1, 2);
    }

    private async Task<double> GetTotalBatteryInputEnergyAsync(CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0x0021;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return Math.Round(data.ToArray()[0] * 0.1, 2);
    }

    private async Task<int> GetModbusPowerControlAsync(CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0x0100;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }

    private async Task<ushort> GetLockStateAsync(CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0x54;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }
    #endregion
}

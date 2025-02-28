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
}

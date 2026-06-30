using System.Text;
using SolaxHub.Domain.Inverter;
using SolaxHub.Infrastructure.Modbus.Client;
using SolaxHub.Infrastructure.Modbus.Registers;

namespace SolaxHub.Infrastructure.Modbus;

internal sealed class InverterRepository : IInverterRepository
{
    private readonly ISolaxModbusClient _client;

    public InverterRepository(ISolaxModbusClient client)
    {
        _client = client;
    }

    public async Task<InverterSnapshot> ReadSnapshotAsync(CancellationToken cancellationToken)
    {
        // Serial number — holding registers, 7 registers = 14 bytes ASCII
        var snData = await _client.ReadHoldingRegistersAsync(HoldingRegisters.SeriesNumber, 7, cancellationToken);
        string serialNumber = Encoding.ASCII.GetString(snData.ToArray());

        // Registration code — holding registers at InputRegisters.RegistrationCodeLan address
        var rcData = await _client.ReadHoldingRegistersAsync(InputRegisters.RegistrationCodeLan, 5, cancellationToken);
        string registrationCode = Encoding.ASCII.GetString(rcData.ToArray());

        // Grid voltage (InverterVoltage)
        var gvData = await _client.ReadInputRegistersAsync(InputRegisters.GridVoltage, 1, cancellationToken);
        ushort inverterVoltage = gvData.ToArray()[0];

        // Grid power (InverterPower)
        var gpData = await _client.ReadInputRegistersAsync(InputRegisters.GridPower, 1, cancellationToken);
        int inverterPower = BitConverter.ToInt16([gpData.Span[1], gpData.Span[0]]);

        // PV voltage 1
        var pvv1Data = await _client.ReadInputRegistersAsync(InputRegisters.PvVoltage1, 1, cancellationToken);
        ushort pvVoltage1 = pvv1Data.ToArray()[0];

        // PV current 1
        var pvc1Data = await _client.ReadInputRegistersAsync(InputRegisters.PvCurrent1, 1, cancellationToken);
        ushort pvCurrent1 = pvc1Data.ToArray()[0];

        // PV voltage 2
        var pvv2Data = await _client.ReadInputRegistersAsync(InputRegisters.PvVoltage2, 1, cancellationToken);
        ushort pvVoltage2 = pvv2Data.ToArray()[0];

        // PV current 2
        var pvc2Data = await _client.ReadInputRegistersAsync(InputRegisters.PvCurrent2, 1, cancellationToken);
        ushort pvCurrent2 = pvc2Data.ToArray()[0];

        // Grid current (phase R / single phase)
        var gcData = await _client.ReadInputRegistersAsync(InputRegisters.GridCurrent, 1, cancellationToken);
        short gridCurrent = BitConverter.ToInt16([gcData.Span[1], gcData.Span[0]]);

        // Grid frequency
        var gfData = await _client.ReadInputRegistersAsync(InputRegisters.GridFrequency, 1, cancellationToken);
        ushort gridFrequency = BitConverter.ToUInt16([gfData.Span[1], gfData.Span[0]]);

        // Inverter temperature
        var itData = await _client.ReadInputRegistersAsync(InputRegisters.InverterTemperature, 1, cancellationToken);
        short inverterTemperature = BitConverter.ToInt16([itData.Span[1], itData.Span[0]]);

        // Radiator temperature
        var rtData = await _client.ReadInputRegistersAsync(InputRegisters.RadiatorTemperature, 1, cancellationToken);
        short radiatorTemperature = BitConverter.ToInt16([rtData.Span[1], rtData.Span[0]]);

        // Run mode (InverterStatus)
        var rmData = await _client.ReadInputRegistersAsync(InputRegisters.RunMode, 1, cancellationToken);
        ushort statusRaw = BitConverter.ToUInt16([rmData.Span[1], rmData.Span[0]]);
        var status = (InverterStatus)(statusRaw + 100);

        // PV power 1
        var pvp1Data = await _client.ReadInputRegistersAsync(InputRegisters.PowerDc1, 1, cancellationToken);
        ushort pvPower1 = BitConverter.ToUInt16([pvp1Data.Span[1], pvp1Data.Span[0]]);

        // PV power 2
        var pvp2Data = await _client.ReadInputRegistersAsync(InputRegisters.PowerDc2, 1, cancellationToken);
        ushort pvPower2 = BitConverter.ToUInt16([pvp2Data.Span[1], pvp2Data.Span[0]]);

        // Battery voltage
        var bvData = await _client.ReadInputRegistersAsync(InputRegisters.BatteryVoltage, 1, cancellationToken);
        short batteryVoltage = BitConverter.ToInt16([bvData.Span[1], bvData.Span[0]]);

        // Battery current
        var bcurData = await _client.ReadInputRegistersAsync(InputRegisters.BatteryCurrent, 1, cancellationToken);
        short batteryCurrent = BitConverter.ToInt16([bcurData.Span[1], bcurData.Span[0]]);

        // Battery power
        var bpData = await _client.ReadInputRegistersAsync(InputRegisters.BatPowerCharge1, 1, cancellationToken);
        int batteryPower = BitConverter.ToInt16([bpData.Span[1], bpData.Span[0]]);

        // Battery capacity
        var bcData = await _client.ReadInputRegistersAsync(InputRegisters.BatteryCapacity, 1, cancellationToken);
        byte batteryCapacity = (byte)BitConverter.ToUInt16([bcData.Span[1], bcData.Span[0]]);

        // Battery output energy total (LSB)
        var botData = await _client.ReadInputRegistersAsync(InputRegisters.OutputEnergyChargeLsb, 1, cancellationToken);
        double batteryOutputTotal = Math.Round(BitConverter.ToUInt16([botData.Span[1], botData.Span[0]]) * 0.1, 2);

        // Battery output energy today
        var bodData = await _client.ReadInputRegistersAsync(InputRegisters.OutputEnergyChargeToday, 1, cancellationToken);
        double batteryOutputToday = Math.Round(BitConverter.ToUInt16([bodData.Span[1], bodData.Span[0]]) * 0.1, 2);

        // Battery input energy total (LSB)
        var bitData = await _client.ReadInputRegistersAsync(InputRegisters.InputEnergyChargeLsb, 1, cancellationToken);
        double batteryInputTotal = Math.Round(BitConverter.ToUInt16([bitData.Span[1], bitData.Span[0]]) * 0.1, 2);

        // Battery input energy today
        var bidData = await _client.ReadInputRegistersAsync(InputRegisters.InputEnergyChargeToday, 1, cancellationToken);
        double batteryInputToday = Math.Round(BitConverter.ToUInt16([bidData.Span[1], bidData.Span[0]]) * 0.1, 2);

        // Feed-in power (2 registers, signed 32-bit, swapped word order: R0=low word, R1=high word)
        var fipData = await _client.ReadInputRegistersAsync(InputRegisters.FeedInPower, 2, cancellationToken);
        int feedInPower = BitConverter.ToInt32([fipData.Span[1], fipData.Span[0], fipData.Span[3], fipData.Span[2]]);

        // Feed-in energy (2 registers, U32 swapped word order, scaled ×0.01)
        var fieData = await _client.ReadInputRegistersAsync(InputRegisters.FeedInEnergy, 2, cancellationToken);
        uint feedInEnergyRaw = (uint)BitConverter.ToInt32([fieData.Span[1], fieData.Span[0], fieData.Span[3], fieData.Span[2]]);
        double feedInEnergy = Math.Round(feedInEnergyRaw * 0.01, 2);

        // Consume energy total (2 registers, U32 swapped word order, scaled ×0.01)
        var ceData = await _client.ReadInputRegistersAsync(InputRegisters.ConsumeEnergyTotal, 2, cancellationToken);
        uint consumeEnergyRaw = (uint)BitConverter.ToInt32([ceData.Span[1], ceData.Span[0], ceData.Span[3], ceData.Span[2]]);
        double consumeEnergy = Math.Round(consumeEnergyRaw * 0.01, 2);

        // Lock state
        var lsData = await _client.ReadInputRegistersAsync(InputRegisters.LockState, 1, cancellationToken);
        var lockState = ((ushort)lsData.ToArray()[1]).ToLockState();

        // Solar charger use mode
        var umData = await _client.ReadInputRegistersAsync(InputRegisters.SolarChargerUseMode, 1, cancellationToken);
        var useMode = ((ushort)umData.ToArray()[1]).ToInverterUseMode();

        // Solar energy total (2 registers, U32 swapped word order, scaled ×0.1)
        var setData = await _client.ReadInputRegistersAsync(InputRegisters.SolarEnergyTotal, 2, cancellationToken);
        uint solarEnergyTotalRaw = (uint)BitConverter.ToInt32([setData.Span[1], setData.Span[0], setData.Span[3], setData.Span[2]]);
        double solarEnergyTotal = Math.Round(solarEnergyTotalRaw * 0.1, 2);

        // Solar energy today (U16 big-endian, scaled ×0.1)
        var sedData = await _client.ReadInputRegistersAsync(InputRegisters.SolarEnergyToday, 1, cancellationToken);
        double solarEnergyToday = Math.Round(BitConverter.ToUInt16([sedData.Span[1], sedData.Span[0]]) * 0.1, 2);

        // Modbus power control mode
        var pcData = await _client.ReadInputRegistersAsync(InputRegisters.ModbusPowerControl, 1, cancellationToken);
        var powerControlMode = (PowerControlMode)pcData.ToArray()[1];

        // Battery temperature (U16, scaled ×0.1)
        var btData = await _client.ReadInputRegistersAsync(InputRegisters.BatteryTemperature, 1, cancellationToken);
        ushort batteryTemperature = BitConverter.ToUInt16([btData.Span[1], btData.Span[0]]);

        // EPS (off-grid / backup) output
        var epsvData = await _client.ReadInputRegistersAsync(InputRegisters.EpsVoltage, 1, cancellationToken);
        ushort epsVoltage = BitConverter.ToUInt16([epsvData.Span[1], epsvData.Span[0]]);
        var epscData = await _client.ReadInputRegistersAsync(InputRegisters.EpsCurrent, 1, cancellationToken);
        ushort epsCurrent = BitConverter.ToUInt16([epscData.Span[1], epscData.Span[0]]);
        var epspData = await _client.ReadInputRegistersAsync(InputRegisters.EpsPower, 1, cancellationToken);
        ushort epsPower = BitConverter.ToUInt16([epspData.Span[1], epspData.Span[0]]);
        var epsfData = await _client.ReadInputRegistersAsync(InputRegisters.EpsFrequency, 1, cancellationToken);
        ushort epsFrequency = BitConverter.ToUInt16([epsfData.Span[1], epsfData.Span[0]]);
        var eps = new EpsState(epsVoltage, epsCurrent, epsPower, epsFrequency);

        // Fault / warning codes (inverter fault is U32 with swapped word order; the rest are U16)
        var ifData = await _client.ReadInputRegistersAsync(InputRegisters.InverterFault, 2, cancellationToken);
        uint inverterFault = (uint)BitConverter.ToInt32([ifData.Span[1], ifData.Span[0], ifData.Span[3], ifData.Span[2]]);
        var cfData = await _client.ReadInputRegistersAsync(InputRegisters.ChargerFault, 1, cancellationToken);
        ushort chargerFault = BitConverter.ToUInt16([cfData.Span[1], cfData.Span[0]]);
        var mfData = await _client.ReadInputRegistersAsync(InputRegisters.ManagerFault, 1, cancellationToken);
        ushort managerFault = BitConverter.ToUInt16([mfData.Span[1], mfData.Span[0]]);
        var bwData = await _client.ReadInputRegistersAsync(InputRegisters.BmsWarning, 1, cancellationToken);
        ushort bmsWarning = BitConverter.ToUInt16([bwData.Span[1], bwData.Span[0]]);
        var faults = new FaultState(inverterFault, chargerFault, managerFault, bmsWarning);

        // Per-phase grid data — three-phase (X3) inverters only.
        GridPhase? phaseR = null, phaseS = null, phaseT = null;
        if (IsThreePhaseSerial(serialNumber))
        {
            phaseR = await ReadGridPhaseAsync(InputRegisters.GridCurrentR, InputRegisters.GridPowerR, cancellationToken);
            phaseS = await ReadGridPhaseAsync(InputRegisters.GridCurrentS, InputRegisters.GridPowerS, cancellationToken);
            phaseT = await ReadGridPhaseAsync(InputRegisters.GridCurrentT, InputRegisters.GridPowerT, cancellationToken);
        }

        return new InverterSnapshot(
            SerialNumber: serialNumber,
            Status: status,
            UseMode: useMode,
            LockState: lockState,
            PowerControlMode: powerControlMode,
            Battery: new BatteryState(batteryPower, batteryCapacity, batteryOutputToday, batteryInputToday, batteryOutputTotal, batteryInputTotal,
                batteryVoltage, batteryCurrent, batteryTemperature),
            Solar: new SolarState(pvVoltage1, pvCurrent1, pvPower1, solarEnergyToday, solarEnergyTotal, pvVoltage2, pvCurrent2, pvPower2),
            Grid: new GridState(feedInPower, feedInEnergy, consumeEnergy, gridFrequency, gridCurrent, phaseR, phaseS, phaseT),
            InverterPower: inverterPower,
            InverterVoltage: inverterVoltage,
            RegistrationCode: registrationCode,
            InverterTemperature: inverterTemperature,
            RadiatorTemperature: radiatorTemperature,
            Eps: eps,
            Faults: faults
        );
    }

    /// <summary>Reads a single grid phase (current + power) for three-phase inverters.</summary>
    private async Task<GridPhase> ReadGridPhaseAsync(ushort currentRegister, ushort powerRegister, CancellationToken cancellationToken)
    {
        var cData = await _client.ReadInputRegistersAsync(currentRegister, 1, cancellationToken);
        short current = BitConverter.ToInt16([cData.Span[1], cData.Span[0]]);
        var pData = await _client.ReadInputRegistersAsync(powerRegister, 1, cancellationToken);
        int power = BitConverter.ToInt16([pData.Span[1], pData.Span[0]]);
        return new GridPhase(current, power);
    }

    /// <summary>Three-phase (X3) inverters have serial prefix "H34"; single-phase (X1) use "H4x"/"H1x".</summary>
    private static bool IsThreePhaseSerial(string serialNumber) => serialNumber.StartsWith("H34");

    public async Task SetLockStateAsync(LockState state, CancellationToken cancellationToken)
    {
        byte[] value = BitConverter.GetBytes((ushort)state);
        await _client.WriteSingleRegisterAsync(WriteRegisters.UnlockPassword, value, cancellationToken);
    }

    public async Task SetUseModeAsync(InverterUseMode mode, CancellationToken cancellationToken)
    {
        await _client.WriteSingleRegisterAsync(WriteRegisters.SolarChargerUseMode,
            BitConverter.GetBytes((ushort)mode), cancellationToken);
    }

    public async Task SetPowerControlAsync(PowerControlMode mode, int chargeDischargePower, CancellationToken cancellationToken)
    {
        var registers = BuildRegisterBlock(mode, chargeDischargePower);
        await _client.WriteMultipleRegistersAsync(WriteRegisters.PowerControl, registers, cancellationToken);
    }

    private static ushort[] BuildRegisterBlock(PowerControlMode mode, int chargeDischargePower)
    {
        const ushort defaultDuration = 60;
        const ushort waitTimeout = 90; // watchdog: inverter exits VPP if no new command arrives within this many seconds

        // Mode 1 (PowerControlMode): GridWTarget at 0x7E–0x7F (LSB first, standard word order).
        //   Positive = import from grid; battery adjusts to cover the gap.
        bool useActivePower = mode == PowerControlMode.PowerControlMode;

        return
        [
            (ushort)mode,                                                                    // 0x7C: VPP mode
            1,                                                                               // 0x7D: TargetSetType=Reset (0=invalid, 1=Reset, 2=Update cumulative)
            useActivePower ? (ushort)(chargeDischargePower & 0xFFFF) : (ushort)0,           // 0x7E: GridWTarget LSB (Mode 1)
            useActivePower ? (ushort)((chargeDischargePower >> 16) & 0xFFFF) : (ushort)0,   // 0x7F: GridWTarget MSB (Mode 1)
            0,                                                                               // 0x80: reactive power LSB
            0,                                                                               // 0x81: reactive power MSB
            defaultDuration,                                                                 // 0x82: ExecDuration (seconds)
            0,                                                                               // 0x83: target SOC
            0,                                                                               // 0x84: target energy LSB
            0,                                                                               // 0x85: target energy MSB
            0,                                                                               // 0x86: unused
            0,                                                                               // 0x87: unused
            waitTimeout,                                                                     // 0x88: WaitTimeout (watchdog, seconds)
        ];
    }

    public async Task SetBatteryMaxDischargeCurrentAsync(double amps, CancellationToken cancellationToken)
    {
        ushort scaledValue = (ushort)Math.Clamp(amps, 0, ushort.MaxValue);
        await _client.WriteSingleRegisterAsync(WriteRegisters.BatteryMaxDischargeCurrent,
            BitConverter.GetBytes(scaledValue), cancellationToken);
    }
}

file static class InverterRepositoryExtensions
{
    public static LockState ToLockState(this ushort value) => value switch
    {
        1 => LockState.Unlocked,
        2 => LockState.UnlockedAdvanced,
        _ => LockState.Locked
    };

    public static InverterUseMode ToInverterUseMode(this ushort value) => value switch
    {
        0 => InverterUseMode.SelfUse,
        1 => InverterUseMode.FeedInPriority,
        2 => InverterUseMode.BackUp,
        3 => InverterUseMode.ForceTimeUse,
        _ => InverterUseMode.Unknown
    };
}

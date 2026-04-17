namespace SolaxHub.Domain.Inverter;

public record InverterSnapshot(
    string SerialNumber,
    InverterStatus Status,
    InverterUseMode UseMode,
    LockState LockState,
    PowerControlMode PowerControlMode,
    BatteryState Battery,
    SolarState Solar,
    GridState Grid,
    int InverterPower,
    ushort InverterVoltage,
    string RegistrationCode
);

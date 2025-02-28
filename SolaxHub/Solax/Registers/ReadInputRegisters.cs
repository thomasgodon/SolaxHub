namespace SolaxHub.Solax.Registers;

public static class ReadInputRegisters
{
    public const ushort GridVoltage = 0x0000;
    public const ushort GridPower = 0x0002;
    public const ushort PvVoltage1 = 0x0003;
    public const ushort PvCurrent1 = 0x0005;
    public const ushort RunMode = 0x0009;
    public const ushort PowerDc1 = 0x000A;
    public const ushort BatPowerCharge1 = 0x0016;
    public const ushort BatteryCapacity = 0x001C;
    public const ushort OutputEnergyChargeLsb = 0x001D;
    public const ushort OutputEnergyChargeToday = 0x0020;
    public const ushort InputEnergyChargeLsb = 0x0021;
    public const ushort InputEnergyChargeToday = 0x0023;
    public const ushort LockState = 0x0054;
    public const ushort FeedInPower = 0x0046;
    public const ushort FeedInEnergy = 0x0048;
    public const ushort ConsumeEnergyTotal = 0x004A;
    public const ushort ModbusPowerControl = 0x0100;
    public const ushort RegistrationCodeLan = 0x00AF;
    public const ushort SolarChargerUseMode = 0x008B;
    public const ushort SolarEnergyToday = 0x0096;
    public const ushort SolarEnergyTotal = 0x0094;
}
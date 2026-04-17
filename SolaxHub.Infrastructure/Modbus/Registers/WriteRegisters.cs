namespace SolaxHub.Infrastructure.Modbus.Registers;

internal static class WriteRegisters
{
    public const ushort UnlockPassword = 0x0000;
    public const ushort SolarChargerUseMode = 0x001F;
    public const ushort BatteryMaxDischargeCurrent = 0x0025;
    public const ushort PowerControl = 0x007C;
    public const ushort Mode8Control = 0x00A0;
}

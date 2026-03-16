using SolaxHub.Domain.Inverter;

namespace SolaxHub.Infrastructure.Knx.Services;

internal sealed class PowerControlBufferService : IPowerControlBufferService
{
    private readonly Lock _lock = new();
    private int _activePower;
    private ushort _duration = 20;
    private byte _targetSoc;
    private int _chargeDischargePower;

    public void SetActivePower(int watts)
    {
        lock (_lock) _activePower = watts;
    }

    public void SetDuration(ushort seconds)
    {
        lock (_lock) _duration = seconds;
    }

    public void SetTargetSoc(byte percent)
    {
        lock (_lock) _targetSoc = percent;
    }

    public void SetChargeDischargePower(int watts)
    {
        lock (_lock) _chargeDischargePower = watts;
    }

    public byte[] BuildRegisterBlock(PowerControlMode mode)
    {
        lock (_lock)
        {
            var data = new byte[30];

            // 0x7C: Power Control Trigger (U16) - mode value
            WriteU16BigEndian(data, 0, (ushort)mode);

            // 0x7D: Reserved (U16)
            WriteU16BigEndian(data, 2, 0);

            // 0x7E-0x7F: Active Power (S32, big-endian word order)
            WriteS32BigEndian(data, 4, _activePower);

            // 0x80-0x81: Reactive Power (S32) - always 0
            WriteS32BigEndian(data, 8, 0);

            // 0x82: Duration (U16)
            WriteU16BigEndian(data, 12, _duration);

            // 0x83: Target SOC (U16)
            WriteU16BigEndian(data, 14, _targetSoc);

            // 0x84-0x85: Target Energy (S32) - always 0
            WriteS32BigEndian(data, 16, 0);

            // 0x86-0x87: Charge/Discharge Power (S32, swapped word order)
            WriteS32SwappedWordOrder(data, 20, _chargeDischargePower);

            // 0x88: Timeout (U16) - always 0
            WriteU16BigEndian(data, 24, 0);

            // 0x89-0x8A: Push Mode Power (S32) - always 0
            WriteS32SwappedWordOrder(data, 26, 0);

            return data;
        }
    }

    private static void WriteU16BigEndian(byte[] buffer, int offset, ushort value)
    {
        buffer[offset] = (byte)(value >> 8);
        buffer[offset + 1] = (byte)(value & 0xFF);
    }

    private static void WriteS32BigEndian(byte[] buffer, int offset, int value)
    {
        buffer[offset] = (byte)(value >> 24);
        buffer[offset + 1] = (byte)(value >> 16);
        buffer[offset + 2] = (byte)(value >> 8);
        buffer[offset + 3] = (byte)(value & 0xFF);
    }

    private static void WriteS32SwappedWordOrder(byte[] buffer, int offset, int value)
    {
        // Low word first, then high word (each word is big-endian)
        buffer[offset] = (byte)(value >> 8);
        buffer[offset + 1] = (byte)(value & 0xFF);
        buffer[offset + 2] = (byte)(value >> 24);
        buffer[offset + 3] = (byte)(value >> 16);
    }
}

namespace SolaxHub.Infrastructure.Modbus.Options;

public class ModbusOptions
{
    public required string Host { get; init; }
    public required int Port { get; init; }
    public TimeSpan PollInterval { get; init; } = TimeSpan.FromSeconds(1);
    public byte UnitIdentifier { get; init; } = 0;
}

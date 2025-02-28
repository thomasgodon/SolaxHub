namespace SolaxHub.Solax.Modbus.Models;

public class SolaxModbusOptions
{
    public required string Host { get; init; }
    public required int Port { get; init; }
    public TimeSpan PollInterval { get; init; } = TimeSpan.FromSeconds(1);
    public required byte UnitIdentifier { get; init; }
}
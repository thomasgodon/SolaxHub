namespace SolaxHub.Solax.Modbus
{
    public class SolaxModbusOptions
    {
        public string Host { get; init; } = default!;
        public int Port { get; init; } = default!;
        public TimeSpan PollInterval { get; init; } = TimeSpan.FromSeconds(1);
    }
}

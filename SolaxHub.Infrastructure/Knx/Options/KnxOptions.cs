namespace SolaxHub.Infrastructure.Knx.Options;

internal class KnxOptions
{
    public bool Enabled { get; set; } = false;
    public string Host { get; set; } = default!;
    public int Port { get; set; } = 3671;
    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(1);

    // Bound as a string so an empty/blank value (e.g. when KNX is disabled) does not throw during
    // configuration binding. Currently informational only — the connection does not use it.
    public string? IndividualAddress { get; set; }
    public Dictionary<string, string> ReadGroupAddresses { get; set; } = default!;
    public Dictionary<string, string> WriteGroupAddresses { get; set; } = default!;
}

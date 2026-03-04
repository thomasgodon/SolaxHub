using Knx.Falcon;

namespace SolaxHub.Infrastructure.Knx.Options;

internal class KnxOptions
{
    public bool Enabled { get; set; } = false;
    public string Host { get; set; } = default!;
    public int Port { get; set; } = 3671;
    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(1);
    public IndividualAddress IndividualAddress { get; set; } = default!;
    public Dictionary<string, string> ReadGroupAddresses { get; set; } = default!;
    public Dictionary<string, string> WriteGroupAddresses { get; set; } = default!;
}

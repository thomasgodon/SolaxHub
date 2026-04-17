namespace SolaxHub.Infrastructure.Udp.Options;

internal class UdpOptions
{
    public bool Enabled { get; set; }
    public string Host { get; set; } = default!;
    public Dictionary<string, int> PortMapping { get; set; } = default!;
}

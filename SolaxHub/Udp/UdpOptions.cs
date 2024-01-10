namespace SolaxHub.Udp
{
    internal class UdpOptions
    {
        public bool Enabled { get; set; } = default!;
        public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(1);
        public string Host { get; set; } = default!;
        public Dictionary<string, int> PortMapping { get; set; } = default!;
    }
}

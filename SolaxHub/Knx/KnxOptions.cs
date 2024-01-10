using Knx.Falcon;

namespace SolaxHub.Knx
{
    internal class KnxOptions
    {
        public bool Enabled { get; set; } = default!;
        public string Host { get; set; } = default!;
        public int Port { get; set; } = default!;
        public IndividualAddress IndividualAddress { get; set; } = default!;
        public Dictionary<string, string> ReadGroupAddresses { get; set; } = default!;
        public Dictionary<string, string> WriteGroupAddresses { get; set; } = default!;
    }
}

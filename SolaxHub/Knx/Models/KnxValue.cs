using Knx.Falcon;

namespace SolaxHub.Knx.Models
{
    internal class KnxSolaxValue
    {
        public KnxSolaxValue(GroupAddress address)
        {
            Address = address;
        }

        public GroupAddress Address { get; }
        public byte[]? Value { get; internal set; }
        public bool IsShort { get; internal set; }

        public override string ToString()
        {
            var value = Value is not null ? string.Join(",", Value.ToList()) : string.Empty;
            return $"{Address} - {value}";
        }
    }
}

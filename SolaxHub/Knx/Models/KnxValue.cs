using Knx.Falcon;

namespace SolaxHub.Knx.Models;

public class KnxValue
{
    public KnxValue(GroupAddress address)
    {
        Address = address;
    }

    public GroupAddress Address { get; }
    public byte[]? Value { get; internal set; }

    public override string ToString()
    {
        string value = Value is not null ? string.Join(",", Value.ToList()) : string.Empty;
        return $"{Address} - {value}";
    }
}
namespace SolaxHub.Knx;

internal class GroupAddressMapping
{
    public GroupAddressMapping(string address, string dataType)
    {
        Address = address;
        DataType = dataType;
    }

    public string Address { get; init; }
    public string DataType { get; init; }
}
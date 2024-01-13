using Knx.Falcon;
using SolaxHub.Knx.Models;

namespace SolaxHub.Knx.Client
{
    internal interface IKnxReadDelegate
    {
        KnxValue? ReadValue(GroupAddress address);
    }
}

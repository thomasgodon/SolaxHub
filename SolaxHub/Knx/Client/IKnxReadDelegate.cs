using Knx.Falcon;
using SolaxHub.Knx.Models;

namespace SolaxHub.Knx.Client
{
    internal interface IKnxReadDelegate
    {
        KnxSolaxValue? ReadValue(GroupAddress address);
    }
}

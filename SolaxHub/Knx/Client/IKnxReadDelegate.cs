using Knx.Falcon;

namespace SolaxHub.Knx.Client
{
    internal interface IKnxReadDelegate
    {
        KnxSolaxValue? ReadValue(GroupAddress address);
    }
}

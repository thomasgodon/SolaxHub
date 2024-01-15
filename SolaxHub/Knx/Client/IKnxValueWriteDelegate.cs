using Knx.Falcon;

namespace SolaxHub.Knx.Client
{
    internal interface IKnxValueWriteDelegate
    {
        Task ProcessValueWriteAsync(GroupAddress groupsAddress, byte[] value, CancellationToken cancellationToken);
    }
}

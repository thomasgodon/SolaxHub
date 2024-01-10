using Knx.Falcon;

namespace SolaxHub.Knx.Client
{
    internal interface IKnxWriteDelegate
    {
        Task ProcessWriteAsync(GroupAddress groupsAddress, byte[] value, CancellationToken cancellationToken);
    }
}

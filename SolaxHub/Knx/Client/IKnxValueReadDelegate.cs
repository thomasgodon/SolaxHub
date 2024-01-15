using Knx.Falcon;
using SolaxHub.Knx.Models;

namespace SolaxHub.Knx.Client
{
    internal interface IKnxValueReadDelegate
    {
        Task<KnxValue?> ResolveValueReadAsync(GroupAddress address, CancellationToken cancellationToken);
    }
}

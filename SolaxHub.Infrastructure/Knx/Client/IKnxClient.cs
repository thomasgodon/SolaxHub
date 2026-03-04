using SolaxHub.Infrastructure.Knx.Models;

namespace SolaxHub.Infrastructure.Knx.Client;

public interface IKnxClient
{
    Task SendValuesAsync(IEnumerable<KnxValue> values, CancellationToken cancellationToken);
    Task ConnectAsync(CancellationToken cancellationToken);
}

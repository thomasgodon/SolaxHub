using SolaxHub.Knx.Models;

namespace SolaxHub.Knx.Client;

public interface IKnxClient
{
    Task SendValuesAsync(IEnumerable<KnxValue> values, CancellationToken cancellationToken);
    Task ConnectAsync(CancellationToken cancellationToken);
}
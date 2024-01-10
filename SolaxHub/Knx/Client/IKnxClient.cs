using SolaxHub.Knx.Models;

namespace SolaxHub.Knx.Client;

internal interface IKnxClient
{
    Task SendValuesAsync(IEnumerable<KnxSolaxValue> values, CancellationToken cancellationToken);
    Task ConnectAsync(CancellationToken cancellationToken);
    void SetReadDelegate(IKnxReadDelegate @delegate);
    void SetWriteDelegate(IKnxWriteDelegate @delegate);
}
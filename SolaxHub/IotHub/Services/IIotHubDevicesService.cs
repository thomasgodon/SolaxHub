using SolaxHub.Solax.Models;

namespace SolaxHub.IotHub.Services;

public interface IIotHubDevicesService
{
    Task SendAsync(SolaxData data, CancellationToken cancellationToken);
}
using SolaxHub.Solax.Models;

namespace SolaxHub.IotHub.Services
{
    internal interface IIotHubDevicesService
    {
        Task Send(SolaxData data, CancellationToken cancellationToken);
    }
}

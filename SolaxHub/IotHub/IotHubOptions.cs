using SolaxHub.IotHub.Models;

namespace SolaxHub.IotHub
{
    internal class IotHubOptions
    {
        public List<IotDevice> IotDevices { get; set; } = default!;
    }
}

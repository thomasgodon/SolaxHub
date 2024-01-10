using SolaxHub.IotCentral.Models;

namespace SolaxHub.IotCentral
{
    internal class IotCentralOptions
    {
        public List<IotDevice> IotDevices { get; set; } = default!;
    }
}

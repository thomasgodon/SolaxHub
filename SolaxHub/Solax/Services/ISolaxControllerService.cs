using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Services
{
    internal interface ISolaxControllerService
    {
        Task SetInverterUseModeAsync(SolaxInverterUseMode useMode, CancellationToken cancellationToken);
    }
}

using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Services
{
    internal interface ISolaxControllerService
    {
        Task SetInverterUseMode(SolaxInverterUseMode useMode);
    }
}

using MediatR;
using SolaxHub.Solax.Models;
using SolaxHub.Solax.Services;

namespace SolaxHub.Solax.Requests.Handlers;

internal class GetChargerUseModeRequestHandler : IRequestHandler<GetChargerUseModeRequest, SolaxInverterUseMode>
{
    private readonly ISolaxControllerService _solaxControllerService;

    public GetChargerUseModeRequestHandler(ISolaxControllerService solaxControllerService)
    {
        _solaxControllerService = solaxControllerService;
    }

    public Task<SolaxInverterUseMode> Handle(GetChargerUseModeRequest request, CancellationToken cancellationToken)
        => Task.FromResult(_solaxControllerService.InverterUseMode);
}

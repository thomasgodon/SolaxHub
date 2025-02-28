using MediatR;
using SolaxHub.Solax.Services;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetInverterVoltageQueryHandler : IRequestHandler<GetInverterVoltageQuery, ushort>
{
    private readonly ISolaxControllerService _solaxControllerService;

    public GetInverterVoltageQueryHandler(ISolaxControllerService solaxControllerService)
    {
        _solaxControllerService = solaxControllerService;
    }

    public async Task<ushort> Handle(GetInverterVoltageQuery request, CancellationToken cancellationToken)
    {
        return await _solaxControllerService.GetInverterVoltageAsync(cancellationToken);
    }
}
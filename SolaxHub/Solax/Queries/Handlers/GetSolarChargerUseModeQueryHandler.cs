namespace SolaxHub.Solax.Queries.Handlers;

public class GetSolarChargerUseModeQueryHandler : IRequestHandler<GetSolarChargerUseModeQuery, ushort>
{
    private readonly ISolaxControllerService _solaxControllerService;

    public GetSolarChargerUseModeQueryHandler(ISolaxControllerService solaxControllerService)
    {
        _solaxControllerService = solaxControllerService;
    }

    public async Task<ushort> Handle(GetSolarChargerUseModeQuery request, CancellationToken cancellationToken)
    {
        return await _solaxControllerService.GetSolarChargerUseModeAsync(cancellationToken);
    }
}
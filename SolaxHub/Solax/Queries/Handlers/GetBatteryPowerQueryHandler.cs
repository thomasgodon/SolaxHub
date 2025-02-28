namespace SolaxHub.Solax.Queries.Handlers;

public class GetBatteryPowerQueryHandler : IRequestHandler<GetBatteryPowerQuery, short>
{
    private readonly ISolaxControllerService _solaxControllerService;

    public GetBatteryPowerQueryHandler(ISolaxControllerService solaxControllerService)
    {
        _solaxControllerService = solaxControllerService;
    }

    public async Task<short> Handle(GetBatteryPowerQuery request, CancellationToken cancellationToken)
    {
        return await _solaxControllerService.GetBatteryPowerAsync(cancellationToken);
    }
}
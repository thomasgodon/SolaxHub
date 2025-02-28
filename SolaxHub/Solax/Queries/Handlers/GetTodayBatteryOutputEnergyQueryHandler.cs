namespace SolaxHub.Solax.Queries.Handlers;

public class GetTodayBatteryOutputEnergyQueryHandler : IRequestHandler<GetTodayBatteryOutputEnergyQuery, double>
{
    private readonly ISolaxControllerService _solaxControllerService;

    public GetTodayBatteryOutputEnergyQueryHandler(ISolaxControllerService solaxControllerService)
    {
        _solaxControllerService = solaxControllerService;
    }

    public async Task<double> Handle(GetTodayBatteryOutputEnergyQuery request, CancellationToken cancellationToken)
    {
        return await _solaxControllerService.GetTodayBatteryOutputEnergyAsync(cancellationToken);
    }
}
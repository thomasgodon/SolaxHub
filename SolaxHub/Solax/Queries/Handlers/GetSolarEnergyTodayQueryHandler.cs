namespace SolaxHub.Solax.Queries.Handlers;

public class GetSolarEnergyTodayQueryHandler : IRequestHandler<GetSolarEnergyTodayQuery, double>
{
    private readonly ISolaxControllerService _solaxControllerService;

    public GetSolarEnergyTodayQueryHandler(ISolaxControllerService solaxControllerService)
    {
        _solaxControllerService = solaxControllerService;
    }

    public async Task<double> Handle(GetSolarEnergyTodayQuery request, CancellationToken cancellationToken)
    {
        return await _solaxControllerService.GetSolarEnergyTodayAsync(cancellationToken);
    }
}
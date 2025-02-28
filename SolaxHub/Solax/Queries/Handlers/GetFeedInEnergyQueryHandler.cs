namespace SolaxHub.Solax.Queries.Handlers;

public class GetFeedInEnergyQueryHandler : IRequestHandler<GetFeedInEnergyQuery, double>
{
    private readonly ISolaxControllerService _solaxControllerService;

    public GetFeedInEnergyQueryHandler(ISolaxControllerService solaxControllerService)
    {
        _solaxControllerService = solaxControllerService;
    }

    public async Task<double> Handle(GetFeedInEnergyQuery request, CancellationToken cancellationToken)
    {
        return await _solaxControllerService.GetFeedInEnergyAsync(cancellationToken);
    }
}
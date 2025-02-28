namespace SolaxHub.Solax.Queries.Handlers;

public class GetFeedInPowerQueryHandler : IRequestHandler<GetFeedInPowerQuery, int>
{
    private readonly ISolaxControllerService _solaxControllerService;

    public GetFeedInPowerQueryHandler(ISolaxControllerService solaxControllerService)
    {
        _solaxControllerService = solaxControllerService;
    }

    public async Task<int> Handle(GetFeedInPowerQuery request, CancellationToken cancellationToken)
    {
        return await _solaxControllerService.GetFeedInPowerAsync(cancellationToken);
    }
}
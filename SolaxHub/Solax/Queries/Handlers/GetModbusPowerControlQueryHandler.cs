namespace SolaxHub.Solax.Queries.Handlers;

public class GetModbusPowerControlQueryHandler : IRequestHandler<GetModbusPowerControlQuery, int>
{
    private readonly ISolaxControllerService _solaxControllerService;

    public GetModbusPowerControlQueryHandler(ISolaxControllerService solaxControllerService)
    {
        _solaxControllerService = solaxControllerService;
    }

    public async Task<int> Handle(GetModbusPowerControlQuery request, CancellationToken cancellationToken)
    {
        return await _solaxControllerService.GetModbusPowerControlAsync(cancellationToken);
    }
}
namespace SolaxHub.Solax.Queries.Handlers;

public class GetInverterStatusQueryHandler : IRequestHandler<GetInverterStatusQuery, ushort>
{
    private readonly ISolaxControllerService _solaxControllerService;

    public GetInverterStatusQueryHandler(ISolaxControllerService solaxControllerService)
    {
        _solaxControllerService = solaxControllerService;
    }

    public async Task<ushort> Handle(GetInverterStatusQuery request, CancellationToken cancellationToken)
    {
        return await _solaxControllerService.GetInverterStatusAsync(cancellationToken);
    }
}
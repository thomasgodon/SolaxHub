namespace SolaxHub.Solax.Queries.Handlers
{
    public class GetPvPower1RQueryHandler : IRequestHandler<GetPvPower1RQuery, ushort>
    {
        private readonly ISolaxControllerService _solaxControllerService;

        public GetPvPower1RQueryHandler(ISolaxControllerService solaxControllerService)
        {
            _solaxControllerService = solaxControllerService;
        }

        public async Task<ushort> Handle(GetPvPower1RQuery request, CancellationToken cancellationToken)
        {
            return await _solaxControllerService.GetPvPower1RAsync(cancellationToken);
        }
    }
}

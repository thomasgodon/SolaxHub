namespace SolaxHub.Solax.Queries.Handlers
{
    public class GetLockStateQueryHandler : IRequestHandler<GetLockStateQuery, ushort>
    {
        private readonly ISolaxControllerService _solaxControllerService;

        public GetLockStateQueryHandler(ISolaxControllerService solaxControllerService)
        {
            _solaxControllerService = solaxControllerService;
        }

        public async Task<ushort> Handle(GetLockStateQuery request, CancellationToken cancellationToken)
        {
            return await _solaxControllerService.GetLockStateAsync(cancellationToken);
        }
    }
}

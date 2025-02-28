namespace SolaxHub.Solax.Queries.Handlers
{
    public class GetPvCurrent1QueryHandler : IRequestHandler<GetPvCurrent1Query, ushort>
    {
        private readonly ISolaxControllerService _solaxControllerService;

        public GetPvCurrent1QueryHandler(ISolaxControllerService solaxControllerService)
        {
            _solaxControllerService = solaxControllerService;
        }

        public async Task<ushort> Handle(GetPvCurrent1Query request, CancellationToken cancellationToken)
        {
            return await _solaxControllerService.GetPvCurrent1Async(cancellationToken);
        }
    }
}

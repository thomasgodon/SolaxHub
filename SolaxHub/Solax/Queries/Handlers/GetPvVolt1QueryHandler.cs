namespace SolaxHub.Solax.Queries.Handlers
{
    public class GetPvVolt1QueryHandler : IRequestHandler<GetPvVolt1Query, ushort>
    {
        private readonly ISolaxControllerService _solaxControllerService;

        public GetPvVolt1QueryHandler(ISolaxControllerService solaxControllerService)
        {
            _solaxControllerService = solaxControllerService;
        }

        public async Task<ushort> Handle(GetPvVolt1Query request, CancellationToken cancellationToken)
        {
            return await _solaxControllerService.GetPvVolt1Async(cancellationToken);
        }
    }
}

namespace SolaxHub.Solax.Queries.Handlers
{
    public class GetInverterPowerQueryHandler : IRequestHandler<GetInverterPowerQuery, short>
    {
        private readonly ISolaxControllerService _solaxControllerService;

        public GetInverterPowerQueryHandler(ISolaxControllerService solaxControllerService)
        {
            _solaxControllerService = solaxControllerService;
        }

        public async Task<short> Handle(GetInverterPowerQuery request, CancellationToken cancellationToken)
        {
            return await _solaxControllerService.GetInverterPowerAsync(cancellationToken);
        }
    }
}

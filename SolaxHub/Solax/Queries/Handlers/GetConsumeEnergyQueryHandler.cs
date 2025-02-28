namespace SolaxHub.Solax.Queries.Handlers
{
    public class GetConsumeEnergyQueryHandler : IRequestHandler<GetConsumeEnergyQuery, double>
    {
        private readonly ISolaxControllerService _solaxControllerService;

        public GetConsumeEnergyQueryHandler(ISolaxControllerService solaxControllerService)
        {
            _solaxControllerService = solaxControllerService;
        }

        public async Task<double> Handle(GetConsumeEnergyQuery request, CancellationToken cancellationToken)
        {
            return await _solaxControllerService.GetConsumeEnergyAsync(cancellationToken);
        }
    }
}

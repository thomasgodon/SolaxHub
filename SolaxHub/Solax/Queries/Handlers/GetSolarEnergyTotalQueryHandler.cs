namespace SolaxHub.Solax.Queries.Handlers
{
    public class GetSolarEnergyTotalQueryHandler : IRequestHandler<GetSolarEnergyTotalQuery, double>
    {
        private readonly ISolaxControllerService _solaxControllerService;

        public GetSolarEnergyTotalQueryHandler(ISolaxControllerService solaxControllerService)
        {
            _solaxControllerService = solaxControllerService;
        }

        public async Task<double> Handle(GetSolarEnergyTotalQuery request, CancellationToken cancellationToken)
        {
            return await _solaxControllerService.GetSolarEnergyTotalAsync(cancellationToken);
        }
    }
}

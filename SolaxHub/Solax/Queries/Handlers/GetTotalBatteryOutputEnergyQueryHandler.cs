namespace SolaxHub.Solax.Queries.Handlers
{
    public class GetTotalBatteryOutputEnergyQueryHandler : IRequestHandler<GetTotalBatteryOutputEnergyQuery, double>
    {
        private readonly ISolaxControllerService _solaxControllerService;

        public GetTotalBatteryOutputEnergyQueryHandler(ISolaxControllerService solaxControllerService)
        {
            _solaxControllerService = solaxControllerService;
        }

        public async Task<double> Handle(GetTotalBatteryOutputEnergyQuery request, CancellationToken cancellationToken)
        {
            return await _solaxControllerService.GetTotalBatteryOutputEnergyAsync(cancellationToken);
        }
    }
}

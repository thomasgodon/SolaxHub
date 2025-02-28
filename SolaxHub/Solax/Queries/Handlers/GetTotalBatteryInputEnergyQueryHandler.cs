namespace SolaxHub.Solax.Queries.Handlers
{
    public class GetTotalBatteryInputEnergyQueryHandler : IRequestHandler<GetTotalBatteryInputEnergyQuery, double>
    {
        private readonly ISolaxControllerService _solaxControllerService;

        public GetTotalBatteryInputEnergyQueryHandler(ISolaxControllerService solaxControllerService)
        {
            _solaxControllerService = solaxControllerService;
        }

        public async Task<double> Handle(GetTotalBatteryInputEnergyQuery request, CancellationToken cancellationToken)
        {
            return await _solaxControllerService.GetTotalBatteryInputEnergyAsync(cancellationToken);
        }
    }
}

namespace SolaxHub.Solax.Queries.Handlers
{
    public class GetTodayBatteryInputEnergyQueryHandler : IRequestHandler<GetTodayBatteryInputEnergyQuery, double>
    {
        private readonly ISolaxControllerService _solaxControllerService;

        public GetTodayBatteryInputEnergyQueryHandler(ISolaxControllerService solaxControllerService)
        {
            _solaxControllerService = solaxControllerService;
        }

        public async Task<double> Handle(GetTodayBatteryInputEnergyQuery request, CancellationToken cancellationToken)
        {
            return await _solaxControllerService.GetTodayBatteryInputEnergyAsync(cancellationToken);
        }
    }
}

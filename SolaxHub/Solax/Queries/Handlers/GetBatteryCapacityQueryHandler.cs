namespace SolaxHub.Solax.Queries.Handlers
{
    public class GetBatteryCapacityQueryHandler : IRequestHandler<GetBatteryCapacityQuery, ushort>
    {
        private readonly ISolaxControllerService _solaxControllerService;

        public GetBatteryCapacityQueryHandler(ISolaxControllerService solaxControllerService)
        {
            _solaxControllerService = solaxControllerService;
        }

        public async Task<ushort> Handle(GetBatteryCapacityQuery request, CancellationToken cancellationToken)
        {
            return await _solaxControllerService.GetBatteryCapacityAsync(cancellationToken);
        }
    }
}

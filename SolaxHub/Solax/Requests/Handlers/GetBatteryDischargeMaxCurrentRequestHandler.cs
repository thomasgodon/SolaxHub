using MediatR;
using SolaxHub.Solax.Services;

namespace SolaxHub.Solax.Requests.Handlers;

internal class GetBatteryDischargeMaxCurrentRequestHandler : IRequestHandler<GetBatteryDischargeMaxCurrentRequest, double>
{
    private readonly ISolaxControllerService _solaxControllerService;

    public GetBatteryDischargeMaxCurrentRequestHandler(ISolaxControllerService solaxControllerService)
    {
        _solaxControllerService = solaxControllerService;
    }

    public Task<double> Handle(GetBatteryDischargeMaxCurrentRequest request, CancellationToken cancellationToken)
    {
        var current = _solaxControllerService.BatteryDischargeLimit;

        return Task.FromResult(2.0);
    }
}

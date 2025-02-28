using MediatR;

namespace SolaxHub.Solax.Requests.Commands;

public class GetBatteryDischargeMaxCurrentRequest : IRequest<double>
{
}

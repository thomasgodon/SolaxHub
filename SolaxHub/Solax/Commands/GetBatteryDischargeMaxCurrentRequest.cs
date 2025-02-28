using MediatR;

namespace SolaxHub.Solax.Commands;

public class GetBatteryDischargeMaxCurrentRequest : IRequest<double>
{
}

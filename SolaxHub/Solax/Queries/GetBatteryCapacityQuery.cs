using MediatR;

namespace SolaxHub.Solax.Queries;

public class GetBatteryCapacityQuery : IRequest<ushort>
{
}
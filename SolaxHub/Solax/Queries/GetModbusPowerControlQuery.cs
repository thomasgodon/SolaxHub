using MediatR;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Queries;

public class GetModbusPowerControlQuery : IRequest<SolaxPowerControlMode>
{
}
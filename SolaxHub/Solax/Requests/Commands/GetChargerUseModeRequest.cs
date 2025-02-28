using MediatR;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Requests.Commands;

internal class GetChargerUseModeRequest : IRequest<SolaxInverterUseMode>
{
}
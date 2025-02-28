using MediatR;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Commands;

internal class GetChargerUseModeRequest : IRequest<SolaxInverterUseMode>
{
}
using MediatR;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Requests;

internal class GetChargerUseModeRequest : IRequest<SolaxInverterUseMode>
{
}
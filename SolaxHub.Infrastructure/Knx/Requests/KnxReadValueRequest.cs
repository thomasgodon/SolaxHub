using Knx.Falcon;
using MediatR;
using SolaxHub.Infrastructure.Knx.Models;

namespace SolaxHub.Infrastructure.Knx.Requests;

internal class KnxReadValueRequest : IRequest<KnxValue?>
{
    public GroupAddress GroupAddress { get; init; }
}

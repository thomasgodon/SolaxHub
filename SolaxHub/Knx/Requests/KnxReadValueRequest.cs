using Knx.Falcon;
using MediatR;
using SolaxHub.Knx.Models;

namespace SolaxHub.Knx.Requests
{
    internal class KnxReadValueRequest : IRequest<KnxValue?>
    {
        public GroupAddress GroupAddress { get; init; }
    }
}

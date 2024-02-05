using Knx.Falcon;
using MediatR;
using SolaxHub.Knx.Models;

namespace SolaxHub.Knx.Requests
{
    internal class KnxInitialReadValueRequest : IRequest<IReadOnlyList<KnxValue>>
    {
    }
}

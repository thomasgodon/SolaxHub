using Knx.Falcon;
using MediatR;
using SolaxHub.Knx.Models;

namespace SolaxHub.Knx.Requests
{
    internal class KnxWriteValueRequest : IRequest
    {
        public GroupAddress GroupAddress { get; init; }
        public byte[] Value { get; init; }
    }
}

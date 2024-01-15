using MediatR;

namespace SolaxHub.Knx.Requests.Handlers
{
    internal class KnxWriteValueRequestHandler : IRequestHandler<KnxWriteValueRequest>
    {
        public Task Handle(KnxWriteValueRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

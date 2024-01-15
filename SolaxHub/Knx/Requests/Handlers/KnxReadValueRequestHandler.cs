using MediatR;
using SolaxHub.Knx.Models;

namespace SolaxHub.Knx.Requests.Handlers
{
    internal class KnxReadValueRequestHandler : IRequestHandler<KnxReadValueRequest, KnxValue?>
    {
        public KnxReadValueRequestHandler()
        {
        }

        public Task<KnxValue?> Handle(KnxReadValueRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

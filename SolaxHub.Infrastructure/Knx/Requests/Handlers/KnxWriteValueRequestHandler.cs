using Knx.Falcon;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SolaxHub.Infrastructure.Knx.Extensions;
using SolaxHub.Infrastructure.Knx.Options;

namespace SolaxHub.Infrastructure.Knx.Requests.Handlers;

internal class KnxWriteValueRequestHandler : IRequestHandler<KnxWriteValueRequest>
{
    private readonly ILogger<KnxWriteValueRequestHandler> _logger;
    private readonly Dictionary<GroupAddress, string> _writeGroupAddressCapabilityMapping;

    public KnxWriteValueRequestHandler(IOptions<KnxOptions> options, ILogger<KnxWriteValueRequestHandler> logger)
    {
        _logger = logger;
        _writeGroupAddressCapabilityMapping = options.Value.GetWriteGroupAddressesFromOptions()
            .ToDictionary(
                m => GroupAddress.Parse(m.Value),
                m => m.Key);
    }

    public Task Handle(KnxWriteValueRequest request, CancellationToken cancellationToken)
    {
        if (!_writeGroupAddressCapabilityMapping.TryGetValue(request.GroupAddress, out var capability))
            return Task.CompletedTask;

        _logger.LogWarning("Writing parameter '{Parameter}' not implemented", capability);
        return Task.CompletedTask;
    }
}

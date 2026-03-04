using Knx.Falcon;
using MediatR;
using Microsoft.Extensions.Options;
using SolaxHub.Infrastructure.Knx.Extensions;
using SolaxHub.Infrastructure.Knx.Models;
using SolaxHub.Infrastructure.Knx.Options;
using SolaxHub.Infrastructure.Knx.Services;

namespace SolaxHub.Infrastructure.Knx.Requests.Handlers;

internal class KnxReadValueRequestHandler : IRequestHandler<KnxReadValueRequest, KnxValue?>
{
    private readonly IKnxValueBufferService _knxValueBufferService;
    private readonly Dictionary<GroupAddress, string> _readGroupAddressCapabilityMapping;

    public KnxReadValueRequestHandler(IOptions<KnxOptions> options, IKnxValueBufferService knxValueBufferService)
    {
        _knxValueBufferService = knxValueBufferService;
        _readGroupAddressCapabilityMapping = options.Value.GetReadGroupAddressesFromOptions()
            .ToDictionary(
                m => GroupAddress.Parse(m.Value),
                m => m.Key);
    }

    public Task<KnxValue?> Handle(KnxReadValueRequest request, CancellationToken cancellationToken)
    {
        if (_readGroupAddressCapabilityMapping.TryGetValue(request.GroupAddress, out var capability) is false)
            return Task.FromResult<KnxValue?>(null);

        return _knxValueBufferService.GetKnxValues().TryGetValue(capability, out var knxValue) is false
            ? Task.FromResult<KnxValue?>(null)
            : Task.FromResult<KnxValue?>(knxValue);
    }
}

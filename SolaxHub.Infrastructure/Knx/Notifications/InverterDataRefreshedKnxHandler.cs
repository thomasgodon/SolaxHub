using MediatR;
using Microsoft.Extensions.Options;
using SolaxHub.Application.Inverter.Notifications;
using SolaxHub.Infrastructure.Knx.Client;
using SolaxHub.Infrastructure.Knx.Options;
using SolaxHub.Infrastructure.Knx.Services;

namespace SolaxHub.Infrastructure.Knx.Notifications;

internal class InverterDataRefreshedKnxHandler : INotificationHandler<InverterDataRefreshed>
{
    private readonly IKnxClient _knxClient;
    private readonly KnxOptions _knxOptions;
    private readonly IKnxValueBufferService _knxValueBufferService;

    public InverterDataRefreshedKnxHandler(
        IKnxClient knxClient,
        IOptions<KnxOptions> options,
        IKnxValueBufferService knxValueBufferService)
    {
        _knxClient = knxClient;
        _knxOptions = options.Value;
        _knxValueBufferService = knxValueBufferService;
    }

    public async Task Handle(InverterDataRefreshed notification, CancellationToken cancellationToken)
    {
        if (_knxOptions.Enabled is false)
            return;

        var values = _knxValueBufferService.UpdateKnxValues(notification.Inverter);
        await _knxClient.SendValuesAsync(values, cancellationToken);
    }
}

using MediatR;
using Microsoft.Extensions.Options;
using SolaxHub.Application.PowerControl.Notifications;
using SolaxHub.Infrastructure.Knx.Client;
using SolaxHub.Infrastructure.Knx.Options;
using SolaxHub.Infrastructure.Knx.Services;

namespace SolaxHub.Infrastructure.Knx.Notifications;

internal class KnxMaxGridImportWattsChangedNotificationHandler : INotificationHandler<MaxGridImportWattsChangedNotification>
{
    private readonly IKnxClient _knxClient;
    private readonly KnxOptions _knxOptions;
    private readonly IKnxValueBufferService _knxValueBufferService;

    public KnxMaxGridImportWattsChangedNotificationHandler(
        IKnxClient knxClient,
        IOptions<KnxOptions> options,
        IKnxValueBufferService knxValueBufferService)
    {
        _knxClient = knxClient;
        _knxOptions = options.Value;
        _knxValueBufferService = knxValueBufferService;
    }

    public async Task Handle(MaxGridImportWattsChangedNotification notification, CancellationToken cancellationToken)
    {
        if (_knxOptions.Enabled is false)
            return;

        var updated = _knxValueBufferService.UpdateMaxGridImportWatts(notification.Watts);
        if (updated is not null)
            await _knxClient.SendValuesAsync([updated], cancellationToken);
    }
}

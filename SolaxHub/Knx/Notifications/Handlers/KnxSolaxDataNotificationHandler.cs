using SolaxHub.Knx.Client;
using MediatR;
using Microsoft.Extensions.Options;
using SolaxHub.Knx.Models;
using SolaxHub.Knx.Services;
using SolaxHub.Solax.Notifications;

namespace SolaxHub.Knx.Notifications.Handlers
{
    internal class KnxSolaxDataNotificationHandler : INotificationHandler<SolaxDataArrivedNotification>
    {
        private readonly IKnxClient _knxClient;
        private readonly KnxOptions _knxOptions;
        private readonly IKnxValueBufferService _knxValueBufferService;

        public KnxSolaxDataNotificationHandler(
            IKnxClient knxClient,
            IOptions<KnxOptions> options,
            IKnxValueBufferService knxValueBufferService)
        {
            _knxClient = knxClient;
            _knxOptions = options.Value;
            _knxValueBufferService = knxValueBufferService;
        }

        public async Task Handle(SolaxDataArrivedNotification notification, CancellationToken cancellationToken)
        {
            if (_knxOptions.Enabled is false)
            {
                return;
            }

            var values = _knxValueBufferService.UpdateKnxValues(notification.Data);
            await _knxClient.SendValuesAsync(values, cancellationToken);
        }
    }
}

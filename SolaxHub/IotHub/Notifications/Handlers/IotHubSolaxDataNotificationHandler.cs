using MediatR;
using SolaxHub.Solax.Notifications;
using SolaxHub.IotHub.Services;

namespace SolaxHub.IotHub.Notifications.Handlers
{
    internal class IotHubSolaxDataNotificationHandler : INotificationHandler<SolaxDataArrivedNotification>
    {
        private readonly IIotHubDevicesService _iotHubDevicesService;

        public IotHubSolaxDataNotificationHandler(IIotHubDevicesService iotHubDevicesService)
        {
            _iotHubDevicesService = iotHubDevicesService;
        }

        public async Task Handle(SolaxDataArrivedNotification notification, CancellationToken cancellationToken)
        {
            await _iotHubDevicesService.Send(notification.Data, cancellationToken);
        }
    }
}

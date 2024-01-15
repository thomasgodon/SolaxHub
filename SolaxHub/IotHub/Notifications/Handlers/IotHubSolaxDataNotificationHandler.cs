using MediatR;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SolaxHub.IotHub.Models;
using SolaxHub.Solax.Notifications;
using System.Text;
using SolaxHub.IotHub.Extensions;

namespace SolaxHub.IotHub.Notifications.Handlers
{
    internal class IotHubSolaxDataNotificationHandler : INotificationHandler<SolaxDataArrivedNotification>
    {
        private readonly IotHubOptions _options;

        public IotHubSolaxDataNotificationHandler(IOptions<IotHubOptions> options)
        {
            _options = options.Value;
        }

        public Task Handle(SolaxDataArrivedNotification notification, CancellationToken cancellationToken)
        {
            while (cancellationToken.IsCancellationRequested is false)
            {
                // process solax data
                var serializedResult = JsonConvert.SerializeObject(notification.Data.ToDeviceData());

                foreach (var (client, interval, deviceOptions) in _deviceClients)
                {
                    if (!deviceOptions.Enabled) return;

                    if (interval.Elapsed < deviceOptions.SendInterval)
                    {
                        continue;
                    }

                    if (_previousResult == serializedResult)
                    {
                        continue;
                    }

                    try
                    {
                        var message = new Message(Encoding.UTF8.GetBytes(serializedResult))
                        {
                            ContentEncoding = Encoding.UTF8.WebName
                        };

                        await client.SendEventAsync(message, cancellationToken);
                        _logger.LogDebug("Send to device with id: {deviceId}", deviceOptions.DeviceId);
                        interval.Restart();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Could not send message to device {deviceId}", deviceOptions.DeviceId);
                    }
                }

                _previousResult = serializedResult;

                // wait for next poll
                // lowest send interval from the configured iot devices is used
                await Task.Delay(_options.IotDevices.Select(m => m.SendInterval).Min(), cancellationToken);
            }
        }
    }
}

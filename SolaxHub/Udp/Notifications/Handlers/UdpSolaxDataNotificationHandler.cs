using MediatR;
using SolaxHub.Solax.Models;
using SolaxHub.Solax.Notifications;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Options;

namespace SolaxHub.Udp.Notifications.Handlers
{
    internal class UdpSolaxDataNotificationHandler : INotificationHandler<SolaxDataArrivedNotification>
    {
        private readonly UdpOptions _options;

        public UdpSolaxDataNotificationHandler(IOptions<UdpOptions> options)
        {
            _options = options.Value;
        }

        public async Task Handle(SolaxDataArrivedNotification notification, CancellationToken cancellationToken)
        {
            // process solax data
            foreach (var (udpData, port) in GenerateUdpMessages(notification.Data, _options.PortMapping).ToList())
            {
                using var udpSender = new UdpClient();
                udpSender.Connect(_options.Host, port);
                await udpSender.SendAsync(udpData, cancellationToken);
            }
        }

        private static IEnumerable<(byte[] Data, int Port)> GenerateUdpMessages(SolaxData data, IReadOnlyDictionary<string, int> portMapping)
        {
            foreach (var propertyInfo in data.GetType().GetProperties())
            {
                dynamic propertyValue;
                try
                {
                    propertyValue = Convert.ChangeType(propertyInfo.GetValue(data, null)?.ToString(), propertyInfo.PropertyType)!;
                }
                catch (Exception)
                {
                    continue;
                }

                if (propertyValue == null)
                {
                    continue;
                }

                if (propertyValue is Enum)
                {
                    propertyValue = (int)propertyValue;
                }

                if (portMapping.TryGetValue(propertyInfo.Name, out var port) is false)
                {
                    continue;
                }

                yield return new ValueTuple<byte[], int>(Encoding.UTF8.GetBytes(propertyValue.ToString()), port);
            }
        }
    }
}

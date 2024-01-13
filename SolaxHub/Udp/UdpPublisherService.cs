using System.Text;
using Microsoft.Extensions.Options;
using SolaxHub.Solax;
using System.Net.Sockets;
using SolaxHub.Solax.Models;

namespace SolaxHub.Udp
{
    internal class UdpPublisherService : ISolaxConsumer
    {
        private readonly UdpOptions _options;

        public UdpPublisherService(IOptions<UdpOptions> udpOptions)
        {
            _options = udpOptions.Value;
        }

        public bool Enabled => _options.Enabled;

        public async Task ConsumeSolaxDataAsync(SolaxData data, CancellationToken cancellationToken)
        {
            if (Enabled is false)
            {
                return;
            }

            // process solax data
            foreach (var (udpData, port) in GenerateUdpMessages(data, _options.PortMapping).ToList())
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

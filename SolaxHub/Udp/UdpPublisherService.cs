using System.Text;
using Microsoft.Extensions.Options;
using SolaxHub.Solax;
using System.Net.Sockets;
using SolaxHub.IotCentral.Models;

namespace SolaxHub.Udp
{
    internal class UdpPublisherService : ISolaxConsumer
    {
        private readonly ISolaxProcessorService _solaxProcessorService;
        private readonly UdpOptions _options;

        public UdpPublisherService(
            IOptions<UdpOptions> udpOptions,
            ISolaxProcessorService solaxProcessorService)
        {
            _solaxProcessorService = solaxProcessorService;
            _options = udpOptions.Value;
        }

        public bool Enabled => _options.Enabled;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (Enabled is false)
            {
                return;
            }

            while (cancellationToken.IsCancellationRequested is false)
            {
                // get solax data
                var data = _solaxProcessorService.ReadSolaxData();

                // process solax data
                foreach (var (udpData, port) in GenerateUdpMessages(data, _options.PortMapping).ToList())
                {
                    using var udpSender = new UdpClient();
                    udpSender.Connect(_options.Host, port);
                    await udpSender.SendAsync(udpData, cancellationToken);
                }

                // wait for next poll
                await Task.Delay(_options.Interval, cancellationToken);
            }
        }

        private static IEnumerable<(byte[] Data, int Port)> GenerateUdpMessages(DeviceData data, Dictionary<string, int> portMapping)
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

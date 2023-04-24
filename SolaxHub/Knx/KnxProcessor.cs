using System.Net;
using Knx.KnxNetIp;
using Knx;
using Knx.DatapointTypes;
using Microsoft.Extensions.Options;
using SolaxHub.Solax;

namespace SolaxHub.Knx
{
    internal class KnxProcessor : ISolaxProcessor
    {
        private readonly ILogger<KnxProcessor> _logger;
        private readonly KnxOptions _knxOptions;
        private readonly KnxNetIpTunnelingClient _client;

        public KnxProcessor(ILogger<KnxProcessor> logger, IOptions<KnxOptions> knxOptions)
        {
            _logger = logger;
            _knxOptions = knxOptions.Value;
            var knxClientEndpoint = new IPEndPoint(IPAddress.Parse(_knxOptions.Host), _knxOptions.Port);
            var knxDeviceAddress = KnxAddress.ParseDevice(_knxOptions.KnxDeviceAddress);
            _client = new KnxNetIpTunnelingClient(knxClientEndpoint, knxDeviceAddress);
        }

        public async Task ProcessData(SolaxData data, CancellationToken cancellationToken)
        {
            if (_knxOptions.Enabled is false) return;

            // connect to the KNXnet/IP gateway
            if (_client.IsConnected is false)
            {
                try
                {
                    await _client.Connect();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Couldn't connect to '{address}'", _client.RemoteEndPoint.Address);
                    await _client.Disconnect();
                }
            }

            foreach (var (address, type) in GenerateKnxMessages(data, _knxOptions.GroupAddressMapping).ToList())
            {
                _client.Write(address, type);
            }
        }

        public static IEnumerable<(KnxLogicalAddress address, DatapointType type)> GenerateKnxMessages(SolaxData data, Dictionary<string, GroupAddressMapping> groupAddressMapping)
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
                    yield break;
                }
                if (propertyValue == null)
                {
                    continue;
                }

                if (groupAddressMapping.TryGetValue(propertyInfo.Name, out var value) is false)
                {
                    continue;
                }

                if (KnxAddress.TryParseLogical(value.Address, out var address) is false)
                {
                    continue;
                }

                if (DatapointType.TryGetType(value.DataType, out var dataPointType) is false)
                {
                    continue;
                }

                var dataPoint = (DatapointType)Activator.CreateInstance(dataPointType, propertyValue)!;

                yield return new ValueTuple<KnxLogicalAddress, DatapointType>(address, dataPoint);
            }
        }
    }
}

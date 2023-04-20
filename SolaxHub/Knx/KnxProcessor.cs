using System.Net;
using Knx.DatapointTypes.Dpt1Bit;
using Knx.ExtendedMessageInterface;
using Knx.KnxNetIp;
using Knx;
using Knx.DatapointTypes;
using Knx.DatapointTypes.Dpt2ByteFloat;
using Microsoft.Extensions.Options;
using SolaxHub.Knx.Extensions;
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
                await _client.Connect();
            }

            foreach (var message in data.ToKnxMessages(KnxAddress.ParseLogical(_knxOptions.StartGroupAddress)))
            {
                _client.Write(message.address, message.type);
            }
        }
    }
}

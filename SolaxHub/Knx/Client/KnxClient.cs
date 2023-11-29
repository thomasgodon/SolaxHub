using Knx.Falcon.Configuration;
using Knx.Falcon.Sdk;
using Knx.Falcon;
using Microsoft.Extensions.Options;

namespace SolaxHub.Knx.Client
{
    internal class KnxClient : IKnxClient
    {
        private readonly ILogger<KnxClient> _logger;
        private readonly KnxOptions _options;
        private KnxBus? _bus;

        public KnxClient(ILogger<KnxClient> logger, IOptions<KnxOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task SendValuesAsync(IEnumerable<KnxSolaxValue> values, CancellationToken cancellationToken)
        {
            if (_bus?.ConnectionState != BusConnectionState.Connected)
            {
                await ConnectAsync(cancellationToken);
            }

            if (_bus == null)
            {
                _logger.LogError("Something went wrong after connecting to knx client");
                return;
            }

            foreach (var knxSolaxValue in values)
            {
                await _bus.WriteGroupValueAsync(knxSolaxValue.Address, new GroupValue(knxSolaxValue.Value), MessagePriority.Low, cancellationToken);
            }
        }

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            if (_bus?.ConnectionState == BusConnectionState.Connected)
            {
                return;
            }

            _bus = new KnxBus(new IpTunnelingConnectorParameters(_options.Host, _options.Port));
            await _bus.ConnectAsync(cancellationToken);
        }
    }
}

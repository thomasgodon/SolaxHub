using Knx.Falcon.Configuration;
using Knx.Falcon.Sdk;
using Knx.Falcon;
using Microsoft.Extensions.Options;
using SolaxHub.Knx.Models;

namespace SolaxHub.Knx.Client
{
    internal class KnxClient : IKnxClient
    {
        private readonly ILogger<KnxClient> _logger;
        private readonly KnxOptions _options;
        private KnxBus? _bus;
        private IKnxValueReadDelegate? _valueReadDelegate;
        private IKnxValueWriteDelegate? _valueWriteDelegate;

        public KnxClient(ILogger<KnxClient> logger, IOptions<KnxOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task SendValuesAsync(IEnumerable<KnxValue> values, CancellationToken cancellationToken)
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
                if (knxSolaxValue.Value is null)
                {
                    return;

                }

                var writeCancellationToken = new CancellationTokenSource();

                await Task.WhenAny(
                    _bus.WriteGroupValueAsync(knxSolaxValue.Address,
                        new GroupValue(knxSolaxValue.Value.Reverse().ToArray()), MessagePriority.Low,
                        writeCancellationToken.Token),
                    Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken));

                writeCancellationToken.Cancel();
            }
        }

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            if (_bus?.ConnectionState == BusConnectionState.Connected)
            {
                return;
            }

            _bus = new KnxBus(new IpTunnelingConnectorParameters(_options.Host, _options.Port));
            _bus.GroupMessageReceived += async (_, args) =>
            {
                await ProcessGroupMessageReceivedAsync(args, cancellationToken);
            };
            await _bus.ConnectAsync(cancellationToken);
        }

        public void SetValueReadDelegate(IKnxValueReadDelegate @delegate)
        {
            _valueReadDelegate = @delegate;
        }

        public void SetValueWriteDelegate(IKnxValueWriteDelegate @delegate)
        {
            _valueWriteDelegate = @delegate;
        }

        private async Task ProcessGroupMessageReceivedAsync(GroupEventArgs e, CancellationToken cancellationToken)
        {
            switch (e.EventType)
            {
                case GroupEventType.ValueRead:
                {
                    if (_valueReadDelegate is null)
                    {
                        return;
                    }
                    var readValue = await _valueReadDelegate.ResolveValueReadAsync(e.DestinationAddress, cancellationToken);
                    if (readValue is null)
                    {
                        return;
                    }

                    await SendValuesAsync(new[] { readValue }, cancellationToken);
                    return;
                }

                case GroupEventType.ValueWrite:
                    if (_valueWriteDelegate is null)
                    {
                        return;
                    }

                    await _valueWriteDelegate.ProcessValueWriteAsync(e.DestinationAddress, e.Value.Value, cancellationToken);
                    break;
                
                default:
                    _logger.LogWarning("Message type'{type}' not implemented", e.EventType);
                        break;
            }
        }
    }
}

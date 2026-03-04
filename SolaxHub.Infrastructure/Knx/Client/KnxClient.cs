using Knx.Falcon;
using Knx.Falcon.Configuration;
using Knx.Falcon.Sdk;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SolaxHub.Infrastructure.Knx.Models;
using SolaxHub.Infrastructure.Knx.Options;
using SolaxHub.Infrastructure.Knx.Requests;
using System.Diagnostics;

namespace SolaxHub.Infrastructure.Knx.Client;

internal class KnxClient : IKnxClient
{
    private static readonly ActivitySource ActivitySource = new(nameof(KnxClient));
    private readonly ILogger<KnxClient> _logger;
    private readonly ISender _sender;
    private readonly KnxOptions _options;
    private KnxBus? _bus;

    public KnxClient(ILogger<KnxClient> logger, IOptions<KnxOptions> options, ISender sender)
    {
        _logger = logger;
        _sender = sender;
        _options = options.Value;
    }

    public async Task SendValuesAsync(IEnumerable<KnxValue> values, CancellationToken cancellationToken)
    {
        using (ActivitySource.StartActivity())
        {
            if (_bus?.ConnectionState != BusConnectionState.Connected)
            {
                _logger.LogWarning("Not connected to '{address}' at port: {port}", _options.Host, _options.Port);
                return;
            }

            foreach (KnxValue knxValue in values)
            {
                if (knxValue.Value is null)
                {
                    _logger.LogError("'{value}' was null for address {address}", nameof(KnxValue.Value), knxValue.Address);
                    continue;
                }

                using var writeCts = new CancellationTokenSource();
                await Task.WhenAny(
                    _bus.WriteGroupValueAsync(knxValue.Address,
                        new GroupValue(knxValue.Value.Reverse().ToArray()), MessagePriority.Low, writeCts.Token),
                    Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken));
                await writeCts.CancelAsync();
            }
        }
    }

    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        using (ActivitySource.StartActivity())
        {
            if (_bus?.ConnectionState == BusConnectionState.Connected)
            {
                _logger.LogTrace("Still connected to '{address}' at port: {port}", _options.Host, _options.Port);
                return;
            }

            _bus = new KnxBus(new IpTunnelingConnectorParameters(_options.Host, _options.Port));
            _bus.GroupMessageReceived += async (_, args) => await ProcessGroupMessageReceivedAsync(args, cancellationToken);
            await _bus.ConnectAsync(cancellationToken);

            if (_bus.ConnectionState == BusConnectionState.Connected)
                _logger.LogInformation("Connected to {host} at port: {port}", _options.Host, _options.Port);
            else
                _logger.LogError("Something went wrong when trying to connect to {host} at port: {port}", _options.Host, _options.Port);
        }
    }

    private async Task ProcessGroupMessageReceivedAsync(GroupEventArgs e, CancellationToken cancellationToken)
    {
        using (ActivitySource.StartActivity())
        {
            switch (e.EventType)
            {
                case GroupEventType.ValueRead:
                    var readValue = await _sender.Send(new KnxReadValueRequest { GroupAddress = e.DestinationAddress }, cancellationToken);
                    if (readValue is not null)
                        await SendValuesAsync([readValue], cancellationToken);
                    return;

                case GroupEventType.ValueWrite:
                    await _sender.Send(new KnxWriteValueRequest { GroupAddress = e.DestinationAddress, Value = e.Value.Value }, cancellationToken);
                    break;

                default:
                    _logger.LogTrace("Message type '{type}' not implemented", e.EventType);
                    break;
            }
        }
    }
}

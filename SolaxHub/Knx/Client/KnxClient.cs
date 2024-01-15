﻿using Knx.Falcon.Configuration;
using Knx.Falcon.Sdk;
using Knx.Falcon;
using MediatR;
using Microsoft.Extensions.Options;
using SolaxHub.Knx.Models;
using SolaxHub.Knx.Requests;

namespace SolaxHub.Knx.Client
{
    internal class KnxClient : IKnxClient
    {
        private readonly ILogger<KnxClient> _logger;
        private readonly ISender _sender;
        private readonly KnxOptions _options;
        private KnxBus? _bus;

        public KnxClient(
            ILogger<KnxClient> logger, 
            IOptions<KnxOptions> options,
            ISender sender)
        {
            _logger = logger;
            _sender = sender;
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

        private async Task ProcessGroupMessageReceivedAsync(GroupEventArgs e, CancellationToken cancellationToken)
        {
            switch (e.EventType)
            {
                case GroupEventType.ValueRead:
                    var readValueRequest = new KnxReadValueRequest()
                    {
                        GroupAddress = e.DestinationAddress
                    };
                    var readValue = await _sender.Send(readValueRequest, cancellationToken);
                    if (readValue is null)
                    {
                        return;
                    }

                    await SendValuesAsync(new[] { readValue }, cancellationToken);
                    return;

                case GroupEventType.ValueWrite:
                    var writeValueRequest = new KnxWriteValueRequest()
                    {
                        GroupAddress = e.DestinationAddress,
                        Value = e.Value.Value
                    };
                    await _sender.Send(writeValueRequest, cancellationToken);
                    break;
                
                default:
                    _logger.LogTrace("Message type'{type}' not implemented", e.EventType);
                        break;
            }
        }
    }
}

using MediatR;
using Microsoft.Extensions.Options;
using SolaxHub.Application.Inverter.Commands.RefreshInverterData;
using SolaxHub.Application.Inverter.Commands.SetInverterLockState;
using SolaxHub.Application.Inverter.Services;
using SolaxHub.Domain.Inverter;
using SolaxHub.Infrastructure.Modbus.Client;
using SolaxHub.Infrastructure.Modbus.Options;
using System.Diagnostics;

namespace SolaxHub.Workers;

internal class InverterPollingWorker : BackgroundService
{
    private static readonly ActivitySource ActivitySource = new(nameof(InverterPollingWorker));
    private readonly ISolaxModbusClient _modbusClient;
    private readonly ISender _sender;
    private readonly IInverterCommandQueue _commandQueue;
    private readonly ModbusOptions _options;
    private readonly ILogger<InverterPollingWorker> _logger;

    public InverterPollingWorker(
        ISolaxModbusClient modbusClient,
        ISender sender,
        IInverterCommandQueue commandQueue,
        IOptions<ModbusOptions> options,
        ILogger<InverterPollingWorker> logger)
    {
        _modbusClient = modbusClient;
        _sender = sender;
        _commandQueue = commandQueue;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested is false)
        {
            try
            {
                if (_modbusClient.IsConnected is false)
                    await ConnectAndUnlockAsync(cancellationToken);

                using (ActivitySource.StartActivity())
                    await _sender.Send(new RefreshInverterDataCommand(), cancellationToken);

                await DrainCommandQueueAsync(cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning("Polling cycle failed ({Message}), retrying in {PollInterval}", ex.Message, _options.PollInterval);
            }

            await Task.Delay(_options.PollInterval, cancellationToken);
        }
    }

    private async Task DrainCommandQueueAsync(CancellationToken cancellationToken)
    {
        while (_commandQueue.TryDequeue(out var command))
            await command!(cancellationToken);
    }

    private async Task ConnectAndUnlockAsync(CancellationToken cancellationToken)
    {
        await _modbusClient.ConnectAsync(cancellationToken);
        await _sender.Send(new SetInverterLockStateCommand(LockState.UnlockedAdvanced), cancellationToken);
    }
}

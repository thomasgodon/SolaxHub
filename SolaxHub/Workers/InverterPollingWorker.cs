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
    private readonly IInverterStateService _inverterStateService;
    private readonly ISender _sender;
    private readonly ModbusOptions _options;

    public InverterPollingWorker(
        ISolaxModbusClient modbusClient,
        IInverterStateService inverterStateService,
        ISender sender,
        IOptions<ModbusOptions> options)
    {
        _modbusClient = modbusClient;
        _inverterStateService = inverterStateService;
        _sender = sender;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested is false)
        {
            using (ActivitySource.StartActivity())
            {
                await _modbusClient.ConnectAsync(cancellationToken);

                if (_inverterStateService.Inverter.LockState != LockState.UnlockedAdvanced)
                    await _sender.Send(new SetInverterLockStateCommand(LockState.UnlockedAdvanced), cancellationToken);

                await _sender.Send(new RefreshInverterDataCommand(), cancellationToken);
            }

            await Task.Delay(_options.PollInterval, cancellationToken);
        }
    }
}

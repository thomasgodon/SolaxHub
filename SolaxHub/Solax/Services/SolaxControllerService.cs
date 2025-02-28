using MediatR;
using SolaxHub.Solax.Commands;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Services;

internal class SolaxControllerService : ISolaxControllerService
{
    private readonly ILogger<SolaxControllerService> _logger;
    private readonly ISolaxModbusClient _solaxModbusClient;
    private readonly ISender _sender;
    private readonly IPublisher _publisher;

    public SolaxControllerService(
        ILogger<SolaxControllerService> logger,
        ISolaxModbusClient solaxModbusClient,
        ISender sender,
        IPublisher publisher)
    {
        _logger = logger;
        _solaxModbusClient = solaxModbusClient;
        _sender = sender;
        _publisher = publisher;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        await _sender.Send(new SetLockStateCommand(SolaxLockState.UnlockedAdvanced), cancellationToken);
    }
}

using MediatR;
using SolaxHub.Solax.Commands;
using SolaxHub.Solax.Models;
using SolaxHub.Solax.Queries;

namespace SolaxHub.Solax.Services;

internal class SolaxPollingService : ISolaxPollingService
{
    private readonly ISender _sender;
    private readonly IPublisher _publisher;

    public SolaxPollingService(ISender sender,
        IPublisher publisher)
    {
        _sender = sender;
        _publisher = publisher;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        SolaxLockState currentLockState = await _sender.Send(new GetLockStateQuery(), cancellationToken);
        if (currentLockState != SolaxLockState.UnlockedAdvanced)
        {
            await _sender.Send(new SetLockStateCommand(SolaxLockState.UnlockedAdvanced), cancellationToken);
        }
    }
}

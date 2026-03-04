using MediatR;
using Microsoft.Extensions.Logging;
using SolaxHub.Application.Inverter.Services;
using SolaxHub.Domain.Common;
using SolaxHub.Domain.Inverter;
using SolaxHub.Domain.Inverter.Events;

namespace SolaxHub.Application.Inverter.Commands.RefreshInverterData;

internal sealed class RefreshInverterDataCommandHandler : IRequestHandler<RefreshInverterDataCommand>
{
    private readonly IInverterRepository _repository;
    private readonly IInverterStateService _stateService;
    private readonly IPublisher _publisher;
    private readonly ILogger<RefreshInverterDataCommandHandler> _logger;

    public RefreshInverterDataCommandHandler(
        IInverterRepository repository,
        IInverterStateService stateService,
        IPublisher publisher,
        ILogger<RefreshInverterDataCommandHandler> logger)
    {
        _repository = repository;
        _stateService = stateService;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task Handle(RefreshInverterDataCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await _repository.ReadSnapshotAsync(cancellationToken);

        _stateService.Inverter.Refresh(snapshot);

        _logger.LogDebug("Inverter refreshed: {SerialNumber}, Status: {Status}, LockState: {LockState}",
            snapshot.SerialNumber, snapshot.Status, snapshot.LockState);

        foreach (var domainEvent in _stateService.Inverter.ClearDomainEvents())
        {
            INotification? notification = MapToNotification(domainEvent);
            if (notification is not null)
                await _publisher.Publish(notification, cancellationToken);
        }
    }

    private static INotification? MapToNotification(IDomainEvent domainEvent) => domainEvent switch
    {
        InverterDataRefreshed e => new Notifications.InverterDataRefreshed(e.Inverter),
        InverterUseModeChanged e => new Notifications.InverterUseModeChanged(e.SerialNumber, e.NewMode),
        _ => null
    };
}

using MediatR;
using Microsoft.Extensions.Logging;
using SolaxHub.Application.PowerControl.Notifications;

namespace SolaxHub.Application.PowerControl.Commands.Handlers;

internal sealed class SetMaxGridImportWattsCommandHandler : IRequestHandler<SetMaxGridImportWattsCommand>
{
    private readonly IPowerControlStateService _stateService;
    private readonly IPublisher _publisher;
    private readonly ILogger<SetMaxGridImportWattsCommandHandler> _logger;

    public SetMaxGridImportWattsCommandHandler(
        IPowerControlStateService stateService,
        IPublisher publisher,
        ILogger<SetMaxGridImportWattsCommandHandler> logger)
    {
        _stateService = stateService;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task Handle(SetMaxGridImportWattsCommand request, CancellationToken cancellationToken)
    {
        _stateService.SetMaxGridImportWatts(request.Watts);
        _logger.LogInformation("Max grid import set to {Watts}W", request.Watts);
        await _publisher.Publish(new MaxGridImportWattsChangedNotification(request.Watts), cancellationToken);
    }
}

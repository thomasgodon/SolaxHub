using MediatR;
using SolaxHub.Domain.Inverter;

namespace SolaxHub.Application.Inverter.Commands.SetInverterUseMode;

internal sealed class SetInverterUseModeCommandHandler : IRequestHandler<SetInverterUseModeCommand>
{
    private readonly IInverterRepository _repository;

    public SetInverterUseModeCommandHandler(IInverterRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(SetInverterUseModeCommand request, CancellationToken cancellationToken)
        => await _repository.SetUseModeAsync(request.Mode, cancellationToken);
}

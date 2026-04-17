using MediatR;
using SolaxHub.Domain.Inverter;

namespace SolaxHub.Application.Inverter.Commands.SetInverterLockState;

internal sealed class SetInverterLockStateCommandHandler : IRequestHandler<SetInverterLockStateCommand>
{
    private readonly IInverterRepository _repository;

    public SetInverterLockStateCommandHandler(IInverterRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(SetInverterLockStateCommand request, CancellationToken cancellationToken)
        => await _repository.SetLockStateAsync(request.LockState, cancellationToken);
}

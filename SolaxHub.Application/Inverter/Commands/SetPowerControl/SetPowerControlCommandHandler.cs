using MediatR;
using SolaxHub.Domain.Inverter;

namespace SolaxHub.Application.Inverter.Commands.SetPowerControl;

internal sealed class SetPowerControlCommandHandler : IRequestHandler<SetPowerControlCommand>
{
    private readonly IInverterRepository _repository;

    public SetPowerControlCommandHandler(IInverterRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(SetPowerControlCommand request, CancellationToken cancellationToken)
        => await _repository.SetPowerControlAsync(request.Mode, request.ChargeDischargePower, cancellationToken);
}

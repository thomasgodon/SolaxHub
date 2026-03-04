using MediatR;
using SolaxHub.Domain.Inverter;

namespace SolaxHub.Application.Inverter.Commands.SetBatteryMaxDischargeCurrent;

internal sealed class SetBatteryMaxDischargeCurrentCommandHandler : IRequestHandler<SetBatteryMaxDischargeCurrentCommand>
{
    private readonly IInverterRepository _repository;

    public SetBatteryMaxDischargeCurrentCommandHandler(IInverterRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(SetBatteryMaxDischargeCurrentCommand request, CancellationToken cancellationToken)
        => await _repository.SetBatteryMaxDischargeCurrentAsync(request.Amps, cancellationToken);
}

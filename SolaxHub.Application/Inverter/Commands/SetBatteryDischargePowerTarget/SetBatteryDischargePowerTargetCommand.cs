using MediatR;

namespace SolaxHub.Application.Inverter.Commands.SetBatteryDischargePowerTarget;

public record SetBatteryDischargePowerTargetCommand(int Watts) : IRequest;

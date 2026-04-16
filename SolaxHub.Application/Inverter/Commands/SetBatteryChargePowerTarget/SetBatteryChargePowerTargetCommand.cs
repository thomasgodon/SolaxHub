using MediatR;

namespace SolaxHub.Application.Inverter.Commands.SetBatteryChargePowerTarget;

public record SetBatteryChargePowerTargetCommand(int Watts) : IRequest;

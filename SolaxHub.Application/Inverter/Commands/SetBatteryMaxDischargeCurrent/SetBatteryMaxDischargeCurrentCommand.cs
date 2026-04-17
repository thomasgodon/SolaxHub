using MediatR;

namespace SolaxHub.Application.Inverter.Commands.SetBatteryMaxDischargeCurrent;

public record SetBatteryMaxDischargeCurrentCommand(double Amps) : IRequest;

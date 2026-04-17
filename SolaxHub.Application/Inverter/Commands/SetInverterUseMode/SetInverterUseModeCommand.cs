using MediatR;
using SolaxHub.Domain.Inverter;

namespace SolaxHub.Application.Inverter.Commands.SetInverterUseMode;

public record SetInverterUseModeCommand(InverterUseMode Mode) : IRequest;

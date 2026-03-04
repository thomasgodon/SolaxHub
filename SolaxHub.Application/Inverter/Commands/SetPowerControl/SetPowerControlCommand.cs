using MediatR;
using SolaxHub.Domain.Inverter;

namespace SolaxHub.Application.Inverter.Commands.SetPowerControl;

public record SetPowerControlCommand(PowerControlMode Mode, byte[] Data) : IRequest;

using MediatR;
using SolaxHub.Domain.Inverter;

namespace SolaxHub.Application.Inverter.Commands.SetInverterLockState;

public record SetInverterLockStateCommand(LockState LockState) : IRequest;

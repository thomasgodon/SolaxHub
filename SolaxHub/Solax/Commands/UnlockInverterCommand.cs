using MediatR;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Commands;

public class UnlockInverterCommand : IRequest
{
    public required SolaxLockState LockState { get; init; }
}
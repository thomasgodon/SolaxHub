using MediatR;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Commands;

public class SetLockStateCommand : IRequest
{
    public SolaxLockState LockState { get; }

    public SetLockStateCommand(SolaxLockState lockState)
    {
        LockState = lockState;
    }
}
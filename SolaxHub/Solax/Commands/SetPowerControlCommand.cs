using MediatR;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Commands;

public class SetPowerControlCommand : IRequest
{
    public SolaxPowerControlMode Mode { get; }
    public byte[] Data { get; }

    public SetPowerControlCommand(SolaxPowerControlMode mode, byte[] data)
    {
        Mode = mode;
        Data = data;
    }
}
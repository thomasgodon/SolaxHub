using MediatR;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Commands;

public class SetSolarChargerUseModeCommand : IRequest
{
    public SolaxInverterUseMode UseMode { get; }

    public SetSolarChargerUseModeCommand(SolaxInverterUseMode useMode)
    {
        UseMode = useMode;
    }
}
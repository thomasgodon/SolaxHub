using MediatR;

namespace SolaxHub.Solax.Commands;

public class SetBatteryDischargeMaxCurrentCommand : IRequest
{
    public double MaxCurrent { get; }

    public SetBatteryDischargeMaxCurrentCommand(double maxCurrent)
    {
        MaxCurrent = maxCurrent;
    }
}
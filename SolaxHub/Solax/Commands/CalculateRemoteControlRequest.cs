using MediatR;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Commands;

internal class CalculateRemoteControlRequest : IRequest<SolaxPowerControlCalculation>
{
    public CalculateRemoteControlRequest(SolaxData solaxData)
    {
        SolaxData = solaxData;
    }

    public SolaxData SolaxData { get; init; }
}
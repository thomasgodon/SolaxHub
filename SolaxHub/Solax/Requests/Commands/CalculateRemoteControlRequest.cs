using MediatR;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Requests.Commands;

internal class CalculateRemoteControlRequest : IRequest<SolaxPowerControlCalculation>
{
    public CalculateRemoteControlRequest(SolaxData solaxData)
    {
        SolaxData = solaxData;
    }

    public SolaxData SolaxData { get; init; }
}
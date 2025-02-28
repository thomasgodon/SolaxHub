using MediatR;
using SolaxHub.Solax.Services;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetSolarEnergyTodayQueryHandler : IRequestHandler<GetSolarEnergyTodayQuery, double>
{
    private readonly ISolaxModbusClient _solaxModbusClient;
    private const byte UnitIdentifier = 0x00;

    public GetSolarEnergyTodayQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<double> Handle(GetSolarEnergyTodayQuery request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0x96;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return Math.Round(data.ToArray()[0] * 0.1, 2);
    }
}

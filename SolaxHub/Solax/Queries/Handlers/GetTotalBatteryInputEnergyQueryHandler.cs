using MediatR;
using SolaxHub.Solax.Services;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetTotalBatteryInputEnergyQueryHandler : IRequestHandler<GetTotalBatteryInputEnergyQuery, double>
{
    private readonly ISolaxModbusClient _solaxModbusClient;
    private const byte UnitIdentifier = 0x00;

    public GetTotalBatteryInputEnergyQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<double> Handle(GetTotalBatteryInputEnergyQuery request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0x0021;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return Math.Round(data.ToArray()[0] * 0.1, 2);
    }
}

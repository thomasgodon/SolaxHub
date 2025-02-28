using MediatR;
using SolaxHub.Solax.Services;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetTotalBatteryOutputEnergyQueryHandler : IRequestHandler<GetTotalBatteryOutputEnergyQuery, double>
{
    private readonly ISolaxModbusClient _solaxModbusClient;
    private const byte UnitIdentifier = 0x00;

    public GetTotalBatteryOutputEnergyQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<double> Handle(GetTotalBatteryOutputEnergyQuery request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0x001D;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return Math.Round(data.ToArray()[0] * 0.1, 2);
    }
}

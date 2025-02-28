using MediatR;
using SolaxHub.Solax.Services;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetBatteryPowerQueryHandler : IRequestHandler<GetBatteryPowerQuery, short>
{
    private readonly ISolaxModbusClient _solaxModbusClient;
    private const byte UnitIdentifier = 0x00;

    public GetBatteryPowerQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<short> Handle(GetBatteryPowerQuery request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 22;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<short>(UnitIdentifier, startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }
}

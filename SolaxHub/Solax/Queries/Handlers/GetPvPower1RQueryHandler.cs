using MediatR;
using SolaxHub.Solax.Services;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetPvPower1RQueryHandler : IRequestHandler<GetPvPower1RQuery, ushort>
{
    private readonly ISolaxModbusClient _solaxModbusClient;
    private const byte UnitIdentifier = 0x00;

    public GetPvPower1RQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<ushort> Handle(GetPvPower1RQuery request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 10;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }
}

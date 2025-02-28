using MediatR;
using SolaxHub.Solax.Modbus.Client;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetPvPower1RQueryHandler : IRequestHandler<GetPvPower1RQuery, ushort>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public GetPvPower1RQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<ushort> Handle(GetPvPower1RQuery request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 10;
        const ushort count = 1;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }
}

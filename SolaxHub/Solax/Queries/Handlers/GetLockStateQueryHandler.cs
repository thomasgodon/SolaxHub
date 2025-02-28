using MediatR;
using SolaxHub.Solax.Services;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetLockStateQueryHandler : IRequestHandler<GetLockStateQuery, ushort>
{
    private readonly ISolaxModbusClient _solaxModbusClient;
    private const byte UnitIdentifier = 0x00;

    public GetLockStateQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<ushort> Handle(GetLockStateQuery request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0x54;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }
}

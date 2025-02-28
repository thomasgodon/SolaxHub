using MediatR;
using SolaxHub.Solax.Services;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetPvVolt1QueryHandler : IRequestHandler<GetPvVolt1Query, ushort>
{
    private readonly ISolaxModbusClient _solaxModbusClient;
    private const byte UnitIdentifier = 0x00;

    public GetPvVolt1QueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<ushort> Handle(GetPvVolt1Query request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 3;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }
}

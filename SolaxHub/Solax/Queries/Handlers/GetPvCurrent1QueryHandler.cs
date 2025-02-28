using MediatR;
using SolaxHub.Solax.Modbus.Client;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetPvCurrent1QueryHandler : IRequestHandler<GetPvCurrent1Query, ushort>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public GetPvCurrent1QueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<ushort> Handle(GetPvCurrent1Query request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 6;
        const ushort count = 1;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }
}

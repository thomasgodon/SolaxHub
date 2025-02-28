using MediatR;
using SolaxHub.Solax.Modbus.Client;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetModbusPowerControlQueryHandler : IRequestHandler<GetModbusPowerControlQuery, int>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public GetModbusPowerControlQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<int> Handle(GetModbusPowerControlQuery request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0x0100;
        const ushort count = 1;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }
}

using MediatR;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Registers;

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
        const ushort quantity = 1;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(ReadInputRegisters.PowerDc1, quantity, cancellationToken);
        return BitConverter.ToUInt16([data.Span[1], data.Span[0]]);
    }
}

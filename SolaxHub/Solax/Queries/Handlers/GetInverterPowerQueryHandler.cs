using MediatR;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Registers;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetInverterPowerQueryHandler : IRequestHandler<GetInverterPowerQuery, short>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public GetInverterPowerQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<short> Handle(GetInverterPowerQuery request, CancellationToken cancellationToken)
    {
        const ushort quantity = 1;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(ReadInputRegisters.GridPower, quantity, cancellationToken);
        return BitConverter.ToInt16([data.Span[1], data.Span[0]]);
    }
}

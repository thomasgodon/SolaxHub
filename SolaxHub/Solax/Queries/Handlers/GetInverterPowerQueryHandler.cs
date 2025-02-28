using MediatR;
using SolaxHub.Solax.Modbus.Client;

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
        const ushort startingAddress = 2;
        const ushort count = 1;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }
}

using MediatR;
using SolaxHub.Solax.Modbus.Client;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetInverterVoltageQueryHandler : IRequestHandler<GetInverterVoltageQuery, ushort>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public GetInverterVoltageQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<ushort> Handle(GetInverterVoltageQuery request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0;
        const ushort count = 1;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }
}
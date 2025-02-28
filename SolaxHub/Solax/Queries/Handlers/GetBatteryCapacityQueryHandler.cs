using MediatR;
using SolaxHub.Solax.Modbus.Client;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetBatteryCapacityQueryHandler : IRequestHandler<GetBatteryCapacityQuery, ushort>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public GetBatteryCapacityQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<ushort> Handle(GetBatteryCapacityQuery request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 28;
        const ushort count = 1;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }
}

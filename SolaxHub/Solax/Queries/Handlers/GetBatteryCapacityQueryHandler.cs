using MediatR;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Registers;

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
        const ushort quantity = 1;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(ReadInputRegisters.BatteryCapacity, quantity, cancellationToken);
        return data.ToArray()[0];
    }
}

using MediatR;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Registers;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetBatteryPowerQueryHandler : IRequestHandler<GetBatteryPowerQuery, short>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public GetBatteryPowerQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<short> Handle(GetBatteryPowerQuery request, CancellationToken cancellationToken)
    {
        const ushort quantity = 1;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(ReadInputRegisters.BatPowerCharge1, quantity, cancellationToken);
        return data.ToArray()[0];
    }
}

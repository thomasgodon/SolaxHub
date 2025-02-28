using MediatR;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetModbusPowerControlQueryHandler : IRequestHandler<GetModbusPowerControlQuery, SolaxPowerControlMode>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public GetModbusPowerControlQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<SolaxPowerControlMode> Handle(GetModbusPowerControlQuery request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0x0100;
        const ushort count = 1;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(startingAddress, count, cancellationToken);
        return SolaxPowerControlMode.Disabled; //data.ToArray()[0];
    }
}

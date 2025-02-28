using MediatR;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Models;
using SolaxHub.Solax.Registers;

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
        const ushort quantity = 1;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(ReadInputRegisters.ModbusPowerControl, quantity, cancellationToken);
        return SolaxPowerControlMode.Disabled; //data.ToArray()[0];
    }
}

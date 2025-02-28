using MediatR;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Registers;

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
        const ushort quantity = 1;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(ReadInputRegisters.GridVoltage, quantity, cancellationToken);
        return data.ToArray()[0];
    }
}
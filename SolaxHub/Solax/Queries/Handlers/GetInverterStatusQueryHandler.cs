using MediatR;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Models;
using SolaxHub.Solax.Registers;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetInverterStatusQueryHandler : IRequestHandler<GetInverterStatusQuery, SolaxInverterStatus>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public GetInverterStatusQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<SolaxInverterStatus> Handle(GetInverterStatusQuery request, CancellationToken cancellationToken)
    {
        const ushort quantity = 1;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(ReadInputRegisters.RunMode, quantity, cancellationToken);
        ushort status = BitConverter.ToUInt16([data.Span[1], data.Span[0]]);
        return (SolaxInverterStatus)(status + 100);
    }
}

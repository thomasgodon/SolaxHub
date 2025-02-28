using MediatR;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Models;

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
        const ushort startingAddress = 9;
        const ushort count = 1;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(startingAddress, count, cancellationToken);
        return SolaxInverterStatus.CheckMode;
    }
}

using MediatR;
using SolaxHub.Solax.Extensions;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Models;
using SolaxHub.Solax.Registers;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetLockStateQueryHandler : IRequestHandler<GetLockStateQuery, SolaxLockState>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public GetLockStateQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<SolaxLockState> Handle(GetLockStateQuery request, CancellationToken cancellationToken)
    {
        const ushort quantity = 1;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(ReadInputRegisters.LockState, quantity, cancellationToken);
        ushort state = data.ToArray()[1];
        return state.ToSolaxLockState();
    }
}

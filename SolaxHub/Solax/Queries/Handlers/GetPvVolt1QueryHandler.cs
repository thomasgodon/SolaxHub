using MediatR;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Registers;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetPvVolt1QueryHandler : IRequestHandler<GetPvVolt1Query, ushort>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public GetPvVolt1QueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<ushort> Handle(GetPvVolt1Query request, CancellationToken cancellationToken)
    {
        const ushort quantity = 1;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(ReadInputRegisters.PvVoltage1, quantity, cancellationToken);
        return data.ToArray()[0];
    }
}

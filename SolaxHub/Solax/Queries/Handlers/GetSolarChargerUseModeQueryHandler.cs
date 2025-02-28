using MediatR;
using SolaxHub.Solax.Modbus.Client;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetSolarChargerUseModeQueryHandler : IRequestHandler<GetSolarChargerUseModeQuery, ushort>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public GetSolarChargerUseModeQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<ushort> Handle(GetSolarChargerUseModeQuery request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0x008B;
        const ushort count = 1;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }
}

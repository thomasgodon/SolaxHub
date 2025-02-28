using MediatR;
using SolaxHub.Solax.Services;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetSolarChargerUseModeQueryHandler : IRequestHandler<GetSolarChargerUseModeQuery, ushort>
{
    private readonly ISolaxModbusClient _solaxModbusClient;
    private const byte UnitIdentifier = 0x00;

    public GetSolarChargerUseModeQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<ushort> Handle(GetSolarChargerUseModeQuery request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0x008B;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadHoldingRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }
}

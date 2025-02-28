using MediatR;
using SolaxHub.Solax.Services;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetInverterPowerQueryHandler : IRequestHandler<GetInverterPowerQuery, short>
{
    private readonly ISolaxModbusClient _solaxModbusClient;
    private const byte UnitIdentifier = 0x00;

    public GetInverterPowerQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<short> Handle(GetInverterPowerQuery request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 2;
        const ushort count = 1;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<short>(UnitIdentifier, startingAddress, count, cancellationToken);
        return data.ToArray()[0];
    }
}

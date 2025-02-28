using MediatR;
using SolaxHub.Solax.Services;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetFeedInPowerQueryHandler : IRequestHandler<GetFeedInPowerQuery, int>
{
    private readonly ISolaxModbusClient _solaxModbusClient;
    private const byte UnitIdentifier = 0x00;

    public GetFeedInPowerQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<int> Handle(GetFeedInPowerQuery request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 70;
        const ushort count = 2;
        var data = await _solaxModbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
        return data.ToArray()[1] << 16 | data.ToArray()[0] & 0xffff;
    }
}

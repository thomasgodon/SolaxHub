using MediatR;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Registers;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetFeedInPowerQueryHandler : IRequestHandler<GetFeedInPowerQuery, int>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public GetFeedInPowerQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<int> Handle(GetFeedInPowerQuery request, CancellationToken cancellationToken)
    {
        const ushort quantity = 2;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(ReadInputRegisters.FeedInPower, quantity, cancellationToken);
        return data.ToArray()[1] << 16 | data.ToArray()[0] & 0xffff;
    }
}

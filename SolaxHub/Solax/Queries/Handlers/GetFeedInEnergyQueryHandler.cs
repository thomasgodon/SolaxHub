using MediatR;
using SolaxHub.Solax.Modbus.Client;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetFeedInEnergyQueryHandler : IRequestHandler<GetFeedInEnergyQuery, double>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public GetFeedInEnergyQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<double> Handle(GetFeedInEnergyQuery request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 72;
        const ushort count = 2;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(startingAddress, count, cancellationToken);
        return Math.Round((data.ToArray()[1] << 16 | data.ToArray()[0] & 0xffff) * 0.01, 2);
    }
}

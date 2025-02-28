using MediatR;
using SolaxHub.Solax.Modbus.Client;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetConsumeEnergyQueryHandler : IRequestHandler<GetConsumeEnergyQuery, double>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public GetConsumeEnergyQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<double> Handle(GetConsumeEnergyQuery request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 74;
        const ushort count = 2;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(startingAddress, count, cancellationToken);
        return Math.Round((data.ToArray()[1] << 16 | data.ToArray()[0] & 0xffff) * 0.01, 2);
    }
}

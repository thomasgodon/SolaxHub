using MediatR;
using SolaxHub.Solax.Modbus.Client;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetSolarEnergyTotalQueryHandler : IRequestHandler<GetSolarEnergyTotalQuery, double>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public GetSolarEnergyTotalQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<double> Handle(GetSolarEnergyTotalQuery request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0x94;
        const ushort count = 2;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(startingAddress, count, cancellationToken);
        return Math.Round((data.ToArray()[1] << 16 | data.ToArray()[0] & 0xffff) * 0.1, 2);
    }
}

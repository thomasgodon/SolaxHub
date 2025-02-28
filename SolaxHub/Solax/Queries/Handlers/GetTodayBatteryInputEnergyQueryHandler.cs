using MediatR;
using SolaxHub.Solax.Modbus.Client;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetTodayBatteryInputEnergyQueryHandler : IRequestHandler<GetTodayBatteryInputEnergyQuery, double>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public GetTodayBatteryInputEnergyQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<double> Handle(GetTodayBatteryInputEnergyQuery request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0x0023;
        const ushort count = 1;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(startingAddress, count, cancellationToken);
        return Math.Round(data.ToArray()[0] * 0.1, 2);
    }
}

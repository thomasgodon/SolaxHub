using MediatR;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Registers;

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
        const ushort quantity = 1;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(ReadInputRegisters.InputEnergyChargeToday, quantity, cancellationToken);
        return Math.Round(data.ToArray()[0] * 0.1, 2);
    }
}

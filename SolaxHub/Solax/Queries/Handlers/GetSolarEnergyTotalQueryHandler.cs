using MediatR;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Registers;

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
        const ushort quantity = 2;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(ReadInputRegisters.SolarEnergyTotal, quantity, cancellationToken);
        return Math.Round((data.ToArray()[1] << 16 | data.ToArray()[0] & 0xffff) * 0.1, 2);
    }
}

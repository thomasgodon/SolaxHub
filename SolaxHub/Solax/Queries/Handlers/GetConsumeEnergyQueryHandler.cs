using MediatR;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Registers;

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
        const ushort quantity = 2;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(ReadInputRegisters.ConsumeEnergyTotal, quantity, cancellationToken);
        return BitConverter.ToInt32([data.Span[1], data.Span[0], data.Span[3], data.Span[2]]);
        //return Math.Round((data.ToArray()[1] << 16 | data.ToArray()[0] & 0xffff) * 0.01, 2);
    }
}

using MediatR;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Registers;
using System.Text;

namespace SolaxHub.Solax.Queries.Handlers;

internal class GetSerialNumberQueryHandler : IRequestHandler<GetSerialNumberQuery, string>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public GetSerialNumberQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<string> Handle(GetSerialNumberQuery request, CancellationToken cancellationToken)
    {
        const ushort quantity = 7;
        Memory<byte> data = await _solaxModbusClient.ReadHoldingRegistersAsync(ReadHoldingRegisters.SeriesNumber, quantity, cancellationToken);
        return Encoding.ASCII.GetString(data.ToArray());
    }
}

using MediatR;
using SolaxHub.Solax.Modbus.Client;
using System.Text;

namespace SolaxHub.Solax.Queries.Handlers;

internal class GetRegistrationCodePocketQueryHandler : IRequestHandler<GetRegistrationCodePocketQuery, string>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public GetRegistrationCodePocketQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<string> Handle(GetRegistrationCodePocketQuery request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 170;
        const ushort count = 5;
        Memory<byte> data = await _solaxModbusClient.ReadHoldingRegistersAsync(startingAddress, count, cancellationToken);
        return Encoding.ASCII.GetString(data.ToArray());
    }
}

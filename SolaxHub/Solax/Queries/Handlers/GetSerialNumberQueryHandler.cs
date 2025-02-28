using MediatR;
using SolaxHub.Solax.Commands.Handlers;
using SolaxHub.Solax.Modbus.Client;
using System.Text;

namespace SolaxHub.Solax.Queries.Handlers;

internal class GetSerialNumberQueryHandler : IRequestHandler<GetSerialNumberQuery, string>
{
    private readonly ISolaxModbusClient _solaxModbusClient;
    private readonly ILogger<UnlockInverterCommandHandler> _logger;

    public GetSerialNumberQueryHandler(
        ISolaxModbusClient solaxModbusClient,
        ILogger<UnlockInverterCommandHandler> logger)
    {
        _solaxModbusClient = solaxModbusClient;
        _logger = logger;
    }

    public async Task<string> Handle(GetSerialNumberQuery request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0;
        const ushort count = 7;
        Memory<byte> data = await _solaxModbusClient.ReadHoldingRegistersAsync(startingAddress, count, cancellationToken);
        return Encoding.ASCII.GetString(data.ToArray());
    }
}

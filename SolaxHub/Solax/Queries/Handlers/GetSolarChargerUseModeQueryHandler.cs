using MediatR;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Queries.Handlers;

public class GetSolarChargerUseModeQueryHandler : IRequestHandler<GetSolarChargerUseModeQuery, SolaxInverterUseMode>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public GetSolarChargerUseModeQueryHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task<SolaxInverterUseMode> Handle(GetSolarChargerUseModeQuery request, CancellationToken cancellationToken)
    {
        const ushort startingAddress = 0x008B;
        const ushort count = 1;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(startingAddress, count, cancellationToken);
        return SolaxInverterUseMode.SelfUseMode; //data.ToArray()[0];
    }
}

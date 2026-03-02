using MediatR;
using SolaxHub.Solax.Extensions;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Models;
using SolaxHub.Solax.Registers;

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
        const ushort quantity = 1;
        Memory<byte> data = await _solaxModbusClient.ReadInputRegistersAsync(ReadInputRegisters.SolarChargerUseMode, quantity, cancellationToken);
        ushort value = data.ToArray()[1];
        return value.ToSolaxInverterUseMode();
    }
}

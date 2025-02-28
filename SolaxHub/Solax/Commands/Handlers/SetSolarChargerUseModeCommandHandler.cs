using MediatR;
using SolaxHub.Solax.Modbus.Client;

namespace SolaxHub.Solax.Commands.Handlers;

public class SetSolarChargerUseModeCommandHandler : IRequestHandler<SetSolarChargerUseModeCommand>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public SetSolarChargerUseModeCommandHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task Handle(SetSolarChargerUseModeCommand request, CancellationToken cancellationToken)
    {
        const ushort registerAddress = 0x001F;
        await _solaxModbusClient.WriteSingleRegisterAsync(registerAddress, BitConverter.GetBytes((ushort)request.UseMode), cancellationToken);
    }
}


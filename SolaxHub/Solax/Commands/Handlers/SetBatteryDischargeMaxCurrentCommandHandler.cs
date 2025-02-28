using MediatR;
using SolaxHub.Solax.Modbus.Client;

namespace SolaxHub.Solax.Commands.Handlers;

public class SetBatteryDischargeMaxCurrentCommandHandler : IRequestHandler<SetBatteryDischargeMaxCurrentCommand>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public SetBatteryDischargeMaxCurrentCommandHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task Handle(SetBatteryDischargeMaxCurrentCommand request, CancellationToken cancellationToken)
    {
        const ushort registerAddress = 0x0025;
        ushort scaledValue = (ushort)Math.Clamp(request.MaxCurrent, 0, ushort.MaxValue);
        await _solaxModbusClient.WriteSingleRegisterAsync(registerAddress, BitConverter.GetBytes(scaledValue), cancellationToken);
    }
}


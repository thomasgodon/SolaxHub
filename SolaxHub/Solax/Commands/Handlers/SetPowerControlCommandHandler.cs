using MediatR;
using SolaxHub.Solax.Modbus.Client;

namespace SolaxHub.Solax.Commands.Handlers;

public class SetPowerControlCommandHandler : IRequestHandler<SetPowerControlCommand>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public SetPowerControlCommandHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task Handle(SetPowerControlCommand request, CancellationToken cancellationToken)
    {
        const ushort registerAddress = 0x7C;
        await _solaxModbusClient.WriteMultipleRegistersAsync(registerAddress, request.Data, cancellationToken);
    }
}


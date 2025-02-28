using MediatR;
using SolaxHub.Solax.Modbus.Client;
using System.Buffers.Binary;

namespace SolaxHub.Solax.Commands.Handlers;

public class SetLockStateCommandHandler : IRequestHandler<SetLockStateCommand>
{
    private readonly ISolaxModbusClient _solaxModbusClient;

    public SetLockStateCommandHandler(ISolaxModbusClient solaxModbusClient)
    {
        _solaxModbusClient = solaxModbusClient;
    }

    public async Task Handle(SetLockStateCommand request, CancellationToken cancellationToken)
    {
        const ushort registerAddress = 0x54;
        await _solaxModbusClient.WriteSingleRegisterAsync(registerAddress, BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((ushort)request.LockState)), cancellationToken);
    }
}


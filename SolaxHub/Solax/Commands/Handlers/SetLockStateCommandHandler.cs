using MediatR;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Registers;
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
        byte[] value = BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((ushort)request.LockState));
        await _solaxModbusClient.WriteSingleRegisterAsync(WriteSingleRegisters.UnlockPassword, value, cancellationToken);
    }
}


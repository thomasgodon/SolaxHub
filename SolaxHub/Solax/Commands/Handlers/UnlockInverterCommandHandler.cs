using MediatR;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Models;
using System.Buffers.Binary;

namespace SolaxHub.Solax.Commands.Handlers;

internal class UnlockInverterCommandHandler : IRequestHandler<UnlockInverterCommand>
{
    private readonly ISolaxModbusClient _solaxModbusClient;
    private readonly ILogger<UnlockInverterCommandHandler> _logger;

    public UnlockInverterCommandHandler(
        ISolaxModbusClient solaxModbusClient,
        ILogger<UnlockInverterCommandHandler> logger)
    {
        _solaxModbusClient = solaxModbusClient;
        _logger = logger;
    }

    public async Task Handle(UnlockInverterCommand request, CancellationToken cancellationToken)
    {
        const ushort registerAddress = 0x0000;
        await _solaxModbusClient.WriteSingleRegisterAsync(registerAddress, BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((ushort)request.LockState)), cancellationToken);

        _logger.LogInformation("Lock state: {LockState}", request.LockState);
    }
}

namespace SolaxHub.Solax.Modbus.Client;

public interface ISolaxModbusClient
{
    bool IsConnected { get; }
    Task ConnectAsync(CancellationToken cancellationToken);
    Task<Memory<byte>> ReadHoldingRegistersAsync(ushort startingAddress, ushort quantity, CancellationToken cancellationToken);
    Task<Memory<byte>> ReadInputRegistersAsync(ushort startingAddress, ushort quantity, CancellationToken cancellationToken);
    Task WriteSingleRegisterAsync(int registerAddress, byte[] dataset, CancellationToken cancellationToken);
    Task WriteMultipleRegistersAsync(ushort startingAddress, byte[] value, CancellationToken cancellationToken);
}
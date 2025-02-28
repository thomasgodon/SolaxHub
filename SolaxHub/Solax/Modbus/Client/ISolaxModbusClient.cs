namespace SolaxHub.Solax.Modbus.Client;

public interface ISolaxModbusClient
{
    bool IsConnected { get; }
    Task ConnectAsync(CancellationToken cancellationToken);
    Task<Memory<byte>> ReadHoldingRegistersAsync(byte unitIdentifier, ushort startingAddress, ushort quantity, CancellationToken cancellationToken);
    Task<Memory<byte>> ReadInputRegistersAsync(byte unitIdentifier, ushort startingAddress, ushort quantity, CancellationToken cancellationToken);
    Task WriteSingleRegisterAsync(int unitIdentifier, int registerAddress, ushort value, CancellationToken cancellationToken);
    Task WriteMultipleRegistersAsync(byte unitIdentifier, ushort startingAddress, byte[] dataset, CancellationToken cancellationToken);
}
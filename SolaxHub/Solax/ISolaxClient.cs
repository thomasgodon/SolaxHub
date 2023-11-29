namespace SolaxHub.Solax;

public interface ISolaxClient
{
    Task Start(CancellationToken cancellationToken);
    Task WriteRegisterAsync(byte identifier, ushort registerAddress, byte[] value, CancellationToken cancellationToken);
}
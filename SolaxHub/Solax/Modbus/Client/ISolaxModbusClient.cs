namespace SolaxHub.Solax.Modbus.Client;

internal interface ISolaxModbusClient
{
    Task Start(CancellationToken cancellationToken);
}
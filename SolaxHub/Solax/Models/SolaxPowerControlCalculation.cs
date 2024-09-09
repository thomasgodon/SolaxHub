namespace SolaxHub.Solax.Models;

internal class SolaxPowerControlCalculation
{

    public SolaxPowerControlCalculation(SolaxPowerControlMode mode, byte[] data)
    {
        Mode = mode;
        Data = data;
    }
    public SolaxPowerControlMode Mode { get; }
    public byte[] Data { get; }

    public static SolaxPowerControlCalculation Disabled()
        => new(SolaxPowerControlMode.Disabled, [0]);
}

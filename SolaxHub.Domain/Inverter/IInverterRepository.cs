namespace SolaxHub.Domain.Inverter;

public interface IInverterRepository
{
    Task<InverterSnapshot> ReadSnapshotAsync(CancellationToken cancellationToken);
    Task SetLockStateAsync(LockState state, CancellationToken cancellationToken);
    Task SetUseModeAsync(InverterUseMode mode, CancellationToken cancellationToken);
    Task SetPowerControlAsync(PowerControlMode mode, int chargeDischargePower, CancellationToken cancellationToken);
    Task SetBatteryMaxDischargeCurrentAsync(double amps, CancellationToken cancellationToken);
}

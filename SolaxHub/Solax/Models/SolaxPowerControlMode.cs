namespace SolaxHub.Solax.Models
{
    internal enum SolaxPowerControlMode
    {
        Disabled = 0,
        PowerControlMode = 1,
        ElectricQuantityTargetControlMode = 2,
        SocTargetControlMode = 3,
        PushPowerPositiveNegativeMode = 4,
        PushPowerZeroMode = 5,
        SelfConsumeChargeDischargeMode = 6,
        SelfConsumeChargeOnlyMode = 7
    }
}
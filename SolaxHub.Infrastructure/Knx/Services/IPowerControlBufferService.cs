using SolaxHub.Domain.Inverter;

namespace SolaxHub.Infrastructure.Knx.Services;

internal interface IPowerControlBufferService
{
    void SetActivePower(int watts);
    void SetDuration(ushort seconds);
    void SetTargetSoc(byte percent);
    void SetChargeDischargePower(int watts);
    byte[] BuildRegisterBlock(PowerControlMode mode);
}

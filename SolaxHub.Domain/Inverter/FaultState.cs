namespace SolaxHub.Domain.Inverter;

/// <summary>Raw fault / warning codes reported by the inverter. Bitfield decoding is firmware-specific and out of scope.</summary>
public record FaultState(
    uint InverterFault,
    ushort ChargerFault,
    ushort ManagerFault,
    ushort BmsWarning
)
{
    /// <summary>True when any fault/warning code is non-zero.</summary>
    public bool HasFault => InverterFault != 0 || ChargerFault != 0 || ManagerFault != 0 || BmsWarning != 0;
}

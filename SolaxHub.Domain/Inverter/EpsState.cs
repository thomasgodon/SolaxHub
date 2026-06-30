namespace SolaxHub.Domain.Inverter;

/// <summary>EPS (off-grid / backup) output values. Raw register units.</summary>
public record EpsState(
    ushort Voltage,   // 0.1V units
    ushort Current,   // 0.1A units
    ushort Power,     // VA
    ushort Frequency  // 0.01Hz units
);

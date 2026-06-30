namespace SolaxHub.Domain.Inverter;

public record GridState(
    int FeedInPower,       // Watts (positive=export, negative=import)
    double FeedInEnergy,   // kWh total exported
    double ConsumeEnergy,  // kWh total imported
    ushort Frequency = 0,  // 0.01Hz units
    short Current = 0,     // 0.1A units (phase R / single phase)
    GridPhase? PhaseR = null, // three-phase only
    GridPhase? PhaseS = null, // three-phase only
    GridPhase? PhaseT = null  // three-phase only
);

/// <summary>Per-phase grid measurements (three-phase inverters only).</summary>
public record GridPhase(
    short Current, // 0.1A units
    int Power      // Watts
);

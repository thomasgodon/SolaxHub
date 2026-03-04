namespace SolaxHub.Domain.Inverter;

public record SolarState(
    ushort Voltage1,    // 0.1V units
    ushort Current1,    // 0.1A units
    ushort Power1,      // Watts
    double EnergyToday, // kWh
    double EnergyTotal  // kWh
);

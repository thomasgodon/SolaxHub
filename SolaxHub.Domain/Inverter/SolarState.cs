namespace SolaxHub.Domain.Inverter;

public record SolarState(
    ushort Voltage1,    // 0.1V units
    ushort Current1,    // 0.1A units
    ushort Power1,      // Watts
    double EnergyToday, // kWh
    double EnergyTotal, // kWh
    ushort Voltage2 = 0,// 0.1V units (PV string 2)
    ushort Current2 = 0,// 0.1A units (PV string 2)
    ushort Power2 = 0   // Watts (PV string 2)
);

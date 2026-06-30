namespace SolaxHub.Domain.Inverter;

public record BatteryState(
    int Power,             // Watts (negative=charge, positive=discharge)
    byte Capacity,         // 0-100%
    double OutputToday,    // kWh
    double InputToday,     // kWh
    double OutputTotal,    // kWh
    double InputTotal,     // kWh
    short Voltage = 0,     // 0.01V units
    short Current = 0,     // 0.1A units (positive=charging)
    ushort Temperature = 0 // 0.1C units
);

namespace SolaxHub.Domain.Inverter;

public record BatteryState(
    int Power,          // Watts (negative=charge, positive=discharge)
    byte Capacity,      // 0-100%
    double OutputToday, // kWh
    double InputToday,  // kWh
    double OutputTotal, // kWh
    double InputTotal   // kWh
);

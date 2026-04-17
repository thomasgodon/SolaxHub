namespace SolaxHub.Domain.Inverter;

public record GridState(
    int FeedInPower,       // Watts (positive=export, negative=import)
    double FeedInEnergy,   // kWh total exported
    double ConsumeEnergy   // kWh total imported
);

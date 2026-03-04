namespace SolaxHub.Application.Inverter.Services;

internal sealed class InverterStateService : IInverterStateService
{
    public Domain.Inverter.Inverter Inverter { get; } = Domain.Inverter.Inverter.Create();
}

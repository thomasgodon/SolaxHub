using SolaxHub.Domain.Inverter;

namespace SolaxHub.Application.Inverter.Services;

public interface IInverterStateService
{
    Domain.Inverter.Inverter Inverter { get; }
}

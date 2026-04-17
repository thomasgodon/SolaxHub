using SolaxHub.Domain.Common;

namespace SolaxHub.Domain.Inverter.Events;

public record InverterDataRefreshed(Inverter Inverter) : IDomainEvent;

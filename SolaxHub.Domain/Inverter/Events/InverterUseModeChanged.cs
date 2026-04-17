using SolaxHub.Domain.Common;

namespace SolaxHub.Domain.Inverter.Events;

public record InverterUseModeChanged(string SerialNumber, InverterUseMode NewMode) : IDomainEvent;

using MediatR;

namespace SolaxHub.Application.Inverter.Notifications;

public record InverterDataRefreshed(Domain.Inverter.Inverter Inverter) : INotification;

using MediatR;
using SolaxHub.Domain.Inverter;

namespace SolaxHub.Application.Inverter.Notifications;

public record InverterUseModeChanged(string SerialNumber, InverterUseMode NewMode) : INotification;

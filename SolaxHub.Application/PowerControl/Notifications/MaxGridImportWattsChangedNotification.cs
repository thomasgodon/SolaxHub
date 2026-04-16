using MediatR;

namespace SolaxHub.Application.PowerControl.Notifications;

public record MaxGridImportWattsChangedNotification(int Watts) : INotification;

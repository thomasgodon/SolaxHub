using MediatR;

namespace SolaxHub.Application.PowerControl.Commands;

public record SetMaxGridImportWattsCommand(int Watts) : IRequest;

using MediatR;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Notifications
{
    internal class SolaxDataArrivedNotification : INotification
    {
        public SolaxDataArrivedNotification(SolaxData data)
        {
            Data = data;
        }

        public SolaxData Data { get; }
    }
}

namespace SolaxHub.Solax.Models
{
    internal class SolaxPowerControlCalculation
    {
        public SolaxPowerControlCalculation(bool modbusPowerControl, double remoteControlActivePower, double remoteControlReactivePower)
        {
            ModbusPowerControl = modbusPowerControl;
            RemoteControlActivePower = remoteControlActivePower;
            RemoteControlReactivePower = remoteControlReactivePower;
        }

        public bool ModbusPowerControl { get; }
        public double RemoteControlActivePower { get; }
        public double RemoteControlReactivePower { get; }
    }
}

using System.Text;
using Microsoft.VisualBasic;

namespace SolaxHub.Solax.Modbus
{
    internal partial class SolaxModbusClient
    {
        private async Task<SolaxModbusData> GetSolaxModbusData(CancellationToken cancellationToken) =>
            new()
            {
                InverterSerialNumber = await GetSerialNumberAsync(cancellationToken),
                GridVoltage = await GetGridVoltageAsync(cancellationToken),
                InverterPower = await GetGridPowerAsync(cancellationToken),
                BatteryCapacity = await GetBatteryCapacityAsync(cancellationToken),
                BatteryPower = await GetBatteryPowerAsync(cancellationToken),
                FeedInPower = await GetFeedInPowerAsync(cancellationToken),
                RunMode = await GetRunModeAsync(cancellationToken),
                EpsVoltR = await GetEpsVoltRAsync(cancellationToken),
                EpsCurrentR = await GetEpsCurrentRAsync(cancellationToken),
                EpsPowerActiveR = await GetEpsPowerActiveRAsync(cancellationToken),
                SolarEnergyToday = await GetSolarEnergyTodayAsync(cancellationToken),
                SolarEnergyTotal = await GetSolarEnergyTotalAsync(cancellationToken),
                SolarChargerUseMode = await GetSolarChargerUseModeAsync(cancellationToken)
            };

        private async Task<string> GetSerialNumberAsync(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 0;
            const ushort count = 7;
            var data = await _modbusClient.ReadHoldingRegistersAsync(UnitIdentifier, startingAddress, count, cancellationToken);
            return Encoding.ASCII.GetString(data.ToArray());
        }

        private async Task<ushort> GetGridVoltageAsync(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 0;
            const ushort count = 1;
            var data = await _modbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
            return data.ToArray()[0];
        }

        private async Task<short> GetGridPowerAsync(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 2;
            const ushort count = 1;
            var data = await _modbusClient.ReadInputRegistersAsync<short>(UnitIdentifier, startingAddress, count, cancellationToken);
            return data.ToArray()[0];
        }

        private async Task<ushort> GetBatteryCapacityAsync(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 28;
            const ushort count = 1;
            var data = await _modbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
            return data.ToArray()[0];
        }

        private async Task<short> GetBatteryPowerAsync(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 22;
            const ushort count = 1;
            var data = await _modbusClient.ReadInputRegistersAsync<short>(UnitIdentifier, startingAddress, count, cancellationToken);
            return data.ToArray()[0];
        }

        private async Task<ushort> GetEpsVoltRAsync(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 118;
            const ushort count = 1;
            var data = await _modbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
            return data.ToArray()[0];
        }

        private async Task<ushort> GetEpsCurrentRAsync(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 119;
            const ushort count = 1;
            var data = await _modbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
            return data.ToArray()[0];
        }

        private async Task<ushort> GetEpsPowerActiveRAsync(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 120;
            const ushort count = 1;
            var data = await _modbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
            return data.ToArray()[0];
        }

        private async Task<ushort> GetSolarEnergyTodayAsync(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 150;
            const ushort count = 1;
            var data = await _modbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
            return data.ToArray()[0];
        }

        private async Task<int> GetSolarEnergyTotalAsync(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 148;
            const ushort count = 2;
            var data = await _modbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
            return data.ToArray()[1] << 16 | data.ToArray()[0];
        }

        private async Task<ushort> GetRunModeAsync(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 9;
            const ushort count = 1;
            var data = await _modbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
            return data.ToArray()[0];
        }

        private async Task<int> GetFeedInPowerAsync(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 70;
            const ushort count = 2;
            var data = await _modbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
            return data.ToArray()[1] << 16 | data.ToArray()[0] & 0xffff;
        }

        private async Task<ushort> GetSolarChargerUseModeAsync(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 139;
            const ushort count = 1;
            var data = await _modbusClient.ReadHoldingRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
            return data.ToArray()[0];
        }
    }
}

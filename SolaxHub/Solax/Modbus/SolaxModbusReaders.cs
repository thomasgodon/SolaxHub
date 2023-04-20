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
                RegistrationCodePocket = await GetRegistrationCodePocketAsync(cancellationToken),
                InverterVoltage = await GetInverterVoltageAsync(cancellationToken),
                InverterPower = await GetInverterPowerAsync(cancellationToken),
                BatteryCapacity = await GetBatteryCapacityAsync(cancellationToken),
                BatteryPower = await GetBatteryPowerAsync(cancellationToken),
                FeedInPower = await GetFeedInPowerAsync(cancellationToken),
                RunMode = await GetRunModeAsync(cancellationToken),
                PvVolt1 = await GetPvVolt1Async(cancellationToken),
                PvCurrent1 = await GetPvCurrent1Async(cancellationToken),
                PvPower1 = await GetPvPower1RAsync(cancellationToken),
                SolarEnergyToday = await GetSolarEnergyTodayAsync(cancellationToken),
                SolarEnergyTotal = await GetSolarEnergyTotalAsync(cancellationToken),
                SolarChargerUseMode = await GetSolarChargerUseModeAsync(cancellationToken),
                ConsumeEnergy = await GetConsumeEnergyAsync(cancellationToken),
                FeedInEnergy = await GetFeedInEnergyAsync(cancellationToken)
            };

        private async Task<string> GetSerialNumberAsync(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 0;
            const ushort count = 7;
            var data = await _modbusClient.ReadHoldingRegistersAsync(UnitIdentifier, startingAddress, count, cancellationToken);
            return Encoding.ASCII.GetString(data.ToArray());
        }

        private async Task<string> GetRegistrationCodePocketAsync(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 170;
            const ushort count = 5;
            var data = await _modbusClient.ReadHoldingRegistersAsync(UnitIdentifier, startingAddress, count, cancellationToken);
            return Encoding.ASCII.GetString(data.ToArray());
        }

        private async Task<ushort> GetInverterVoltageAsync(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 0;
            const ushort count = 1;
            var data = await _modbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
            return data.ToArray()[0];
        }

        private async Task<short> GetInverterPowerAsync(CancellationToken cancellationToken)
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

        private async Task<ushort> GetPvVolt1Async(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 3;
            const ushort count = 1;
            var data = await _modbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
            return data.ToArray()[0];
        }

        private async Task<ushort> GetPvCurrent1Async(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 6;
            const ushort count = 1;
            var data = await _modbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
            return data.ToArray()[0];
        }

        private async Task<ushort> GetPvPower1RAsync(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 10;
            const ushort count = 1;
            var data = await _modbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
            return data.ToArray()[0];
        }

        private async Task<double> GetSolarEnergyTodayAsync(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 80;
            const ushort count = 1;
            var data = await _modbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
            return Math.Round(data.ToArray()[0] * 0.1, 2);
        }

        private async Task<double> GetSolarEnergyTotalAsync(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 82;
            const ushort count = 2;
            var data = await _modbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
            return Math.Round((data.ToArray()[1] << 16 | data.ToArray()[0] & 0xffff) * 0.1, 2);
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

        private async Task<double> GetFeedInEnergyAsync(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 72;
            const ushort count = 2;
            var data = await _modbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
            return Math.Round((data.ToArray()[1] << 16 | data.ToArray()[0] & 0xffff) * 0.01, 2);
        }

        private async Task<double> GetConsumeEnergyAsync(CancellationToken cancellationToken)
        {
            const ushort startingAddress = 74;
            const ushort count = 2;
            var data = await _modbusClient.ReadInputRegistersAsync<ushort>(UnitIdentifier, startingAddress, count, cancellationToken);
            return Math.Round((data.ToArray()[1] << 16 | data.ToArray()[0] & 0xffff) * 0.01, 2);
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

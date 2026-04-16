# SolaxHub

SolaxHub is a .NET 9 Worker Service that bridges a **Solax solar inverter** (via Modbus TCP) to **KNX** home automation, **Azure IoT Hub**, and **UDP**. It polls the inverter on a configurable interval, publishes the data to all enabled integrations, and accepts power-control commands from KNX or the console.

## Configuration

All settings live in `appsettings.json`.

### Modbus (inverter connection)

```json
"ModbusOptions": {
  "Host": "192.168.1.100",
  "Port": 502,
  "UnitIdentifier": 1,
  "PollInterval": "0.00:00:01"
}
```

## KNX Integration

Enable KNX and fill in your tunnel connection details:

```json
"KnxOptions": {
  "Enabled": true,
  "Host": "192.168.1.10",
  "Port": 3671,
  "IndividualAddress": "1.1.100"
}
```

### Read group addresses

SolaxHub writes these values to the KNX bus every poll cycle (only when the value has changed).

| Key | Description | Encoding |
|-----|-------------|----------|
| `HouseLoad` | House power consumption | 4-byte IEEE 754 float, W |
| `InverterPower` | Inverter AC output power | 4-byte IEEE 754 float, W |
| `BatteryPower` | Battery power (positive = discharge) | 4-byte IEEE 754 float, W |
| `BatteryCapacity` | State of charge | 1-byte scaled: `value × 2.55` → 0–255 (DPT 5.001) |
| `InverterUseMode` | Current use mode | 1-byte scaled: `enum × 2.55` → 0–255 (DPT 5.001) |
| `PvPower1` | Solar string 1 power | 4-byte IEEE 754 float, W |
| `ConsumeEnergy` | Cumulative grid consumption | 4-byte IEEE 754 float, kWh |
| `InverterStatus` | Inverter status code | 1-byte raw enum |
| `SolarEnergyToday` | Solar energy generated today | 4-byte IEEE 754 float, kWh |
| `SolarEnergyTotal` | Total solar energy generated | 4-byte IEEE 754 float, kWh |
| `BatteryOutputEnergyToday` | Battery discharge energy today | 4-byte IEEE 754 float, kWh |
| `BatteryInputEnergyToday` | Battery charge energy today | 4-byte IEEE 754 float, kWh |
| `BatteryOutputEnergyTotal` | Total battery discharge energy | 4-byte IEEE 754 float, kWh |
| `BatteryInputEnergyTotal` | Total battery charge energy | 4-byte IEEE 754 float, kWh |
| `PowerControl` | Active power control mode | 1-byte raw enum |
| `LockState` | Inverter lock state | 1-byte |
| `MaxGridImportWatts` | Current max grid import limit | 4-byte IEEE 754 float, W |

Configure each key with a KNX group address string, or leave blank to skip it:

```json
"ReadGroupAddresses": {
  "HouseLoad": "1/0/1",
  "InverterPower": "1/0/2",
  "BatteryCapacity": "1/0/3"
}
```

### Write group addresses

SolaxHub listens for KNX `ValueWrite` telegrams on these addresses and sends the corresponding command to the inverter.

| Key | Description | Encoding |
|-----|-------------|----------|
| `InverterUseMode` | Set inverter use mode | 1-byte enum value (see table below) |
| `BatteryDischargePowerTarget` | Set battery discharge power | 4-byte IEEE 754 float, W — send `0` to disable |
| `BatteryChargePowerTarget` | Set battery charge power from grid | 4-byte IEEE 754 float, W — send `0` to disable |
| `MaxGridImportWatts` | Set max grid import limit for battery charging | 4-byte IEEE 754 float, W — defaults to `0` at startup |

```json
"WriteGroupAddresses": {
  "InverterUseMode": "1/1/1",
  "BatteryDischargePowerTarget": "1/1/2",
  "BatteryChargePowerTarget": "1/1/3"
}
```

#### InverterUseMode byte values

| Value | Mode |
|-------|------|
| `0` | Self Use |
| `1` | Force Time Use |
| `2` | Back Up |
| `3` | Feed In Priority |

### Power control behaviour

**Discharge (`BatteryDischargePowerTarget`)**
Puts the inverter into grid-target mode. SolaxHub calculates how much the battery actually needs to contribute given current solar production and house load, then adjusts the grid target accordingly. Sending `0` returns the inverter to its previous use mode.

**Charge (`BatteryChargePowerTarget`)**
Puts the inverter into battery-target mode to pull power from the grid. The actual charge rate is capped so that total grid import (house load + charging) never exceeds `MaxGridImportWatts`. Sending `0` disables power control.

**`MaxGridImportWatts`**
Sets the hard cap on total grid draw used by the charge calculation. Defaults to `0` at startup (charging disabled until explicitly set). Configure the `MaxGridImportWatts` write group address and write the limit in watts before issuing charge commands.

Both commands are adaptive — they re-calculate on the latest inverter snapshot each poll cycle.

---

## Console commands

When running interactively, SolaxHub accepts these commands:

```
set discharge <watts>   Force battery discharge at target rate (0 = disable)
set charge <watts>      Charge battery from grid at target rate (0 = disable)
set mode <name>         Switch inverter mode: self-use | feed-in | backup | force-time | solar-only
```

`solar-only` is a special mode that disables grid charging and battery discharge simultaneously.

---

## Build & run

```bash
dotnet build SolaxHub.sln
dotnet run --project SolaxHub/SolaxHub.csproj

# Publish for Raspberry Pi
dotnet publish SolaxHub/SolaxHub.csproj -r linux-arm64 -c Release
```

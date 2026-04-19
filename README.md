# SolaxHub

SolaxHub is a .NET Worker Service that bridges a **Solax solar inverter** (via Modbus TCP) to **KNX** home automation, **Azure IoT Hub**, and **UDP**. It polls the inverter on a configurable interval, publishes the data to all enabled integrations, and accepts power-control commands from KNX or the console.

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

| Key | Description | DPT | Encoding |
|-----|-------------|-----|----------|
| `HouseLoad` | House power consumption | 14.056 | 4-byte IEEE 754 float, W |
| `InverterPower` | Inverter AC output power | 14.056 | 4-byte IEEE 754 float, W |
| `BatteryPower` | Battery power (positive = discharge) | 14.056 | 4-byte IEEE 754 float, W |
| `BatteryCapacity` | State of charge | 5.001 | 1-byte scaled: `value × 2.55` → 0–255 |
| `InverterUseMode` | Current use mode | 5.010 | 1-byte raw enum (see table below) |
| `PvPower1` | Solar string 1 power | 14.056 | 4-byte IEEE 754 float, W |
| `ConsumeEnergy` | Cumulative grid consumption | 13.010 | 4-byte signed integer, Wh |
| `InverterStatus` | Inverter status code | 5.010 | 1-byte raw enum |
| `SolarEnergyToday` | Solar energy generated today | 13.010 | 4-byte signed integer, Wh |
| `SolarEnergyTotal` | Total solar energy generated | 13.010 | 4-byte signed integer, Wh |
| `BatteryOutputEnergyToday` | Battery discharge energy today | 13.010 | 4-byte signed integer, Wh |
| `BatteryInputEnergyToday` | Battery charge energy today | 13.010 | 4-byte signed integer, Wh |
| `BatteryOutputEnergyTotal` | Total battery discharge energy | 13.010 | 4-byte signed integer, Wh |
| `BatteryInputEnergyTotal` | Total battery charge energy | 13.010 | 4-byte signed integer, Wh |
| `PowerControlMode` | Active power control mode | 5.010 | 1-byte raw enum |
| `LockState` | Inverter lock state (0 = locked, 1 = unlocked, 2 = unlocked advanced) | 5.010 | 1-byte |

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

| Key | Description | DPT | Encoding |
|-----|-------------|-----|----------|
| `InverterUseMode` | Set inverter use mode | 5.010 | 1-byte enum value (see table below) |
| `PowerControlMode` | Set VPP power control mode | 5.010 | 1-byte integer: `0` = Disabled, `1`–`12` = VPP mode number |
| `PowerControlPowerTarget` | Set VPP power target | 14.056 | 4-byte IEEE 754 float, W |

```json
"WriteGroupAddresses": {
  "InverterUseMode": "1/1/1",
  "PowerControlMode": "1/1/2",
  "PowerControlPowerTarget": "1/1/3"
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

**VPP power control (`PowerControlMode` + `PowerControlPowerTarget`)**

Direct VPP (Virtual Power Plant) control via Solax register block (0x7C–0x88). Commands are **not stored in EEPROM** — safe for frequent writes.

Flow:
1. Write a mode number (`1`–`12`) to `PowerControlMode` to arm the controller.
2. Write the desired watt target (float) to `PowerControlPowerTarget` — stored in `IPowerControlStateService`.
3. SolaxHub **automatically re-sends** the command every poll cycle. No client-side loop needed.
4. Write `0` to `PowerControlMode` to disable — SolaxHub sends a Disabled command immediately and stops re-sending.

Mode determines the meaning of the watt target:

| Mode | Name | Target parameter |
|------|------|-----------------|
| 0 | Disabled | — |
| 1 | Power Control Mode | `GridWTarget` — grid port active power (positive = import, negative = export) |
| 5 | Push Power Zero Mode | *(no target needed)* |
| 6 | Self-Consume Charge/Discharge | *(no target needed)* |
| 7 | Self-Consume Charge Only | *(no target needed)* |
| 12 | Max Output Mode | *(no target needed)* |

Additional modes (4, 8, 9, 11) will be supported in future updates.

---

## Console commands

When running interactively, SolaxHub accepts these commands:

```
set power-control <mode> [watts]   Set VPP power control mode (re-sent every poll cycle)
set use-mode <name>                Set inverter use mode: self-use | feed-in | backup | force-time | solar-only
```

### `set power-control`

Sets the VPP mode and (for mode 1) the watt target. The command is re-sent to the inverter **every poll cycle** automatically — the inverter stays in the requested mode without any client-side keep-alive loop.

```
set power-control 0            Disable power control (immediate + stops periodic sending)
set power-control 1 500        Mode 1: GridWTarget = 500W (import from grid)
set power-control 1 -800       Mode 1: GridWTarget = -800W (export to grid)
set power-control 7            Mode 7: Self-Consume Charge Only (no watts needed)
```

### `set use-mode`

Sets the inverter's base use mode (stored in the inverter, persists across reboots).

```
set use-mode self-use      Self-consumption
set use-mode feed-in       Feed-in priority
set use-mode backup        Backup / reserve mode
set use-mode force-time    Force time use (TOU scheduling)
set use-mode solar-only    Self-consume charge only (no battery discharge)
```

---

## Build & run

```bash
dotnet build SolaxHub.sln
dotnet run --project SolaxHub/SolaxHub.csproj

# Publish for Raspberry Pi
dotnet publish SolaxHub/SolaxHub.csproj -r linux-arm64 -c Release
```

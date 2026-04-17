# Solax VPP Remote Power Control

VPP (Virtual Power Plant) allows external systems to dynamically control the inverter's power routing via Modbus. Commands are **NOT stored in EEPROM** — safe for frequent writes. Commands auto-revert when expired.

---

## Mode Overview

| Mode | Official Name | Control Point | Legacy? |
|---|---|---|---|
| 0 | VPP Disable | — | — |
| 1 | Power Control Mode | Grid port active power | No |
| 2 | Electric Quantity Target Control Mode | Grid port energy (kWh) | No |
| 3 | SOC Target Control Mode | Battery SOC | No |
| 4 | Push Power - Positive/Negative Mode | Battery charge/discharge power | Yes (10min default) |
| 5 | Push Power - Zero Mode | Battery = 0 (string inverter mode) | Yes (10min default) |
| 6 | Self-Consume - Charge/Discharge Mode | Self-Use emulation | Yes (10min default) |
| 7 | Self-Consume - Charge Only Mode | Self-Use, no discharge | Yes (10min default) |
| 8 | PV&BAT Individual Setting - Duration Mode | PV + Battery independently | No |
| 9 | PV&BAT Individual Setting - Target SOC Mode | PV + Battery until SOC | No |
| 11 | Max Input Mode | Stop PV, charge battery at max from grid | No |
| 12 | Max Output Mode | Maximize PV + max battery discharge to grid | No |

Mode 10 does not exist in the protocol.

---

## Mode Descriptions

### Mode 0: VPP Disable
VPP is inactive. Inverter follows its configured use mode.

### Mode 1: Power Control Mode
Controls the **grid port active power**. PV runs at maximum output. Battery adjusts to satisfy the grid port target.
- **Positive GridWTarget** = inverter absorbs from grid (imports)
- **Negative GridWTarget** = inverter outputs to grid (exports)
- Three sub-scenarios based on PV and battery availability. For details see Solax KB.

### Mode 2: Electric Quantity Target Control Mode
Targets a specific energy quantity (Wh) to import or export via the grid port. PV runs at maximum. Inverter tracks accumulated energy and stops when target is met. If timeout is reached before the target, the device exits VPP.
- `TargetSetType = 0x0001`: Reset — resets accumulated value when a new command arrives
- `TargetSetType = 0x0002`: Update — preserves accumulated value, only updates the target

### Mode 3: SOC Target Control Mode
Drives the battery to a target State of Charge percentage. PV runs at maximum. If mode requires grid power, grid port power can be specified. Mode exits when SOC target is reached, ExecDuration expires, or WaitTimeout fires.
- **Note:** Reportedly buggy on some firmware versions — may revert to Mode 6 (Self-Consume) unexpectedly.

### Mode 4: Push Power - Positive/Negative Mode *(Legacy)*
**Directly controls battery charge/discharge power.** PV runs at maximum. Grid adjusts as a result of battery + PV + load balance.
- **Positive BatWTarget** = battery discharges
- **Negative BatWTarget** = battery charges
- Legacy mode: default 10-minute duration. No ExecDuration or WaitTimeout parameters. Exits to Mode 6 (Self-Consume) by default.

### Mode 5: Push Power - Zero Mode *(Legacy)*
Battery power = 0. PV runs at maximum. Grid absorbs or provides the remainder. **The inverter behaves like a string inverter** — batteries remain powered but do not work. This is NOT a zero-export mode.
- Legacy mode: default 10-minute duration. Exits to Mode 6 by default.

### Mode 6: Self-Consume - Charge/Discharge Mode *(Legacy)*
Emulates Self-Use mode via remote control. Allows both battery charge and discharge.
- Legacy mode: default 10-minute duration. This is also the default exit target for modes 4-7.

### Mode 7: Self-Consume - Charge Only Mode *(Legacy)*
Like Self-Use but battery is **NOT allowed to discharge**. Battery can only be charged by PV (not from grid). Surplus PV goes to the grid.
- Legacy mode: default 10-minute duration. Exits to Mode 6 by default.

### Mode 8: PV&BAT Individual Setting - Duration Mode
Simultaneously and independently controls PV power limit and battery charge/discharge power for a fixed time duration. Priority: battery target first, then PV maximized toward its limit. PV limit can be set to 0 (allowing grid import to charge battery).

### Mode 9: PV&BAT Individual Setting - Target SOC Mode
Same as Mode 8 but exits when the target SOC is reached rather than when a duration expires.

### Mode 11: Max Input Mode
Stops PV output entirely. Charges battery at maximum charging power from the grid. Exits when ExecDuration expires, then switches to next motion (Mode 6 or exit VPP).

### Mode 12: Max Output Mode
Maximizes PV output at rated power. Discharges battery at maximum power. Both feed into the grid simultaneously. Exits when ExecDuration expires.

---

## Register Layouts

### Two APIs: Legacy (0x7C block) and New (0xA0 block)

The KB article documents two APIs:
- **Legacy API (0x7C block):** Modes 0-7 only. Older devices. Some 3rd-party integrations use this.
- **New API (0xA0 block):** All modes 1-12. Recommended for all new implementations.

Both use Function Code 0x10 (FC16 — Write Multiple Registers). Values are NOT stored in EEPROM.

---

### Legacy API: Register 0x7C Block (Modes 0-7)

**Mode 0 (Disable):** Write 0x0000 to register 0x7C only.

**Mode 1 (Power Control):**

| Register | Name | Type | Unit | Description |
|---|---|---|---|---|
| 0x7C | VPPModeNum | U16 | — | 0x0001 |
| 0x7D | TargetSetType | U16 | — | 0x0001=Reset, 0x0002=Update cumulative |
| 0x7E (LSB) + 0x7F (MSB) | GridWTarget | S32 | W | Grid port active power. Positive=import, negative=export |
| 0x80 (LSB) + 0x81 (MSB) | GridVarTarget | S32 | VAR | Reactive power. Positive=inductive, negative=capacitive |
| 0x82 | ExecDuration | U16 | s | Mode execution time |
| 0x88 | WaitTimeout | U16 | s | Watchdog: exits VPP if no new command arrives |

**Mode 2 (Electric Quantity Target):**

| Register | Name | Type | Unit | Description |
|---|---|---|---|---|
| 0x7C | VPPModeNum | U16 | — | 0x0002 |
| 0x7D | TargetSetType | U16 | — | 0x0001=Reset, 0x0002=Update cumulative |
| 0x84 (LSB) + 0x85 (MSB) | GridWhTarget | U32 | Wh | Grid port energy target |
| 0x86 (LSB) + 0x87 (MSB) | GridWTarget | S32 | W | Grid port active power while in mode |
| 0x88 | WaitTimeout | U16 | s | Watchdog timeout |

**Mode 3 (SOC Target):**

| Register | Name | Type | Unit | Description |
|---|---|---|---|---|
| 0x7C | VPPModeNum | U16 | — | 0x0003 |
| 0x7D | TargetSetType | U16 | — | 0x0001=Reset, 0x0002=Update cumulative |
| 0x83 | SocTarget | U16 | % | Target battery SOC |
| 0x86 (LSB) + 0x87 (MSB) | GridWTarget | S32 | W | Grid port active power |
| 0x88 | WaitTimeout | U16 | s | Watchdog timeout |

**Mode 4 (Push Power Positive/Negative) — Legacy:**

| Register | Name | Type | Unit | Description |
|---|---|---|---|---|
| 0x7C | VPPModeNum | U16 | — | 0x0004 |
| 0x89 (LSB) + 0x8A (MSB) | BatWTarget | S32 | W | Battery power. Positive=discharge, negative=charge. **Swapped word order** |

**Modes 5-7 (Legacy) — only VPPModeNum needed:**

| Register | Name | Value |
|---|---|---|
| 0x7C | VPPModeNum | 0x0005 / 0x0006 / 0x0007 |

No additional registers for modes 5-7. Default 10-minute duration applies.

---

### New API: Register 0xA0 Block (All Modes 1-12)

This is the unified, recommended API. Register 0xA0 = VPPModeNum for any mode number.

**Mode 1 (Power Control) — example layout:**

| Register | Name | Type | Description |
|---|---|---|---|
| 0xA0 | VPPModeNum | U16 | 0x0001 |
| 0xA1 | TargetSetType | U16 | 0x0001=Reset, 0x0002=Update |
| 0xA2 (LSB) + 0xA3 (MSB) | GridWTarget | S32 | Grid port active power (W). Positive=import, negative=export |
| 0xA4 (LSB) + 0xA5 (MSB) | GridVarTarget | S32 | Reactive power (VAR) |
| 0xA6 | ExecDuration | U16 | Duration (s) |
| 0xA7 | WaitTimeout | U16 | Watchdog timeout (s) |

**Modes 8-9 layout:**

| Register | Name | Type | Description |
|---|---|---|---|
| 0xA0 | VPPModeNum | U16 | 0x0008 (Duration) or 0x0009 (SOC) |
| 0xA1 | TargetSetType | U16 | 0x0001=Reset, 0x0002=Update |
| 0xA2 (LSB) + 0xA3 (MSB) | PVWTarget | U32 | PV power limit (W). Set to 0 to stop PV |
| 0xA4 (LSB) + 0xA5 (MSB) | BatWTarget | S32 | Battery power (W). Positive=discharge, negative=charge |
| 0xA6 | ExecDuration / SocTarget | U16 | Duration in s (Mode 8) or SOC % (Mode 9) |
| 0xA7 | WaitTimeout | U16 | Watchdog timeout (s) |

**Modes 11-12 layout (use 0xA0 block with ExecDuration and WaitTimeout).**

---

## Word Order Note

Registers with (LSB, MSB) pairs in the 0x7C and 0xA0 blocks use **swapped word order**: the low word (LSB) is at the lower register address, the high word (MSB) at the higher address. This is the opposite of standard Modbus big-endian convention.

Confirmed swapped registers: 0x86-0x87, 0x89-0x8A, 0xA2-0xA3, 0xA4-0xA5.

Decode: `value = (msb_register << 16) | (lsb_register & 0xFFFF)`

---

## Key VPP Behavior

- **Duration**: ExecDuration sets how long a command runs. Must be repeatedly sent before it expires.
- **WaitTimeout**: Watchdog. If no new command arrives within WaitTimeout seconds, VPP exits.
- **Modes 4-7 (legacy)**: Default 10-minute lifetime. Auto-exit to Mode 6.
- **Autorepeat**: Some integrations implement automatic re-sending during each polling cycle.
- **Firmware minimum**: DSP v1.52, ARM v1.50 for full VPP support.
- **Reaction speed**: As fast as 0.092 seconds.
- **VPP overrides** the base use mode temporarily; reverts when command expires.
- **EEPROM**: VPP commands are NOT written to EEPROM — safe for frequent automation.
- **ActivePower relationship (legacy docs):** `ActivePower = BatteryChargePower - PVPower`

---

## VPP Parameters Reference

| Parameter | Range | Description |
|---|---|---|
| GridWTarget | −30,000 to +30,000 W | Grid port power (negative=export, positive=import) |
| GridVarTarget | −4,000 to +4,000 VAR | Reactive power |
| GridWhTarget | 0 to max U32 Wh | Energy quantity target |
| SocTarget | 0–100 % | Target battery SOC |
| BatWTarget | −30,000 to +30,000 W | Battery power (positive=discharge, negative=charge) |
| PVWTarget | 0–30,000 W | PV power limit |
| ExecDuration | 0–28,800 s | Mode execution window |
| WaitTimeout | 0–28,800 s | Watchdog timeout |

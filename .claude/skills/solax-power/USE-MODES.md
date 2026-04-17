# Solax Inverter Use Modes

Use modes (working modes) define the inverter's energy routing strategy. They are stored in EEPROM — limit writes to ~7 per day to preserve lifespan (~100,000 write cycles). For frequent automation, use VPP modes instead.

---

## Inverter Status States (Input Register 0x09 / RunMode)

| Value | State | Operational Meaning |
|---|---|---|
| 0 | **Wait Mode** | Waiting for proper conditions (grid voltage, PV voltage). No power conversion. Common at night or during grid anomalies. |
| 1 | **Check Mode** | Verifying grid parameters, relay states, and system health before transitioning to Normal. |
| 2 | **Normal Mode** | Active grid-tied operation. Inverter is converting power per the selected use mode. |
| 3 | **Fault Mode** | Recoverable fault detected. Power conversion stopped. May auto-clear. |
| 4 | **Permanent Fault** | Non-recoverable hardware fault. Requires service intervention. |
| 5 | **Update Mode** | Firmware update in progress. Do NOT power off. |
| 6 | **EPS Check Mode** | Verifying conditions before entering off-grid/EPS mode. |
| 7 | **EPS Mode** | Off-grid operation. Grid has failed. PV + Battery supply EPS loads. |
| 8 | **Self-Test** | Internal diagnostics running. |
| 9 | **Idle Mode** | On but not converting. Occurs when battery at min SOC, no PV, no charge window. |

Normal operation cycle: Wait → Check → Normal. At night: Normal → Idle. During outage: EPS Check → EPS.

---

## Use Mode Register Mapping

**Important:** The value-to-mode mapping DIFFERS between read and write registers!

### Write (Holding Register 0x1F)

| Value | Mode |
|---|---|
| 0 | Self Use |
| 1 | Force Time Use |
| 2 | Back Up |
| 3 | Feed-In Priority |

### Read (Input Register 0x8B)

| Value | Mode |
|---|---|
| 0 | Self Use |
| 1 | Feed-In Priority |
| 2 | Back Up |
| 3 | Force Time Use |

Feed-In Priority is written as 3 but read back as 1. Force Time Use is written as 1 but read back as 3. Always use the correct register and mapping for read vs. write.

---

## Use Mode Descriptions

### Self Use (Write: 0)
**Energy routing:** PV → House Load → Battery (charge) → Grid (export). When PV insufficient: Battery (discharge if above min SOC) → Grid (import).
**Priority:** Solar > Battery > Grid.
Default minimum SOC: 10%.

### Force Time Use / TOU (Write: 1)
Time-of-use scheduling. Up to 10 date intervals, each with a configurable sub-mode. Supports weekday/weekend schedules.

**TOU Sub-modes:**
| Sub-mode | Behavior |
|---|---|
| Self-Use | Standard self-consumption within this period |
| Battery Hold | Battery inactive except when below min SOC |
| Peak-Shaving | Battery discharges only when load exceeds peak threshold |
| Charging | Aggressive battery charging; optional grid charging; configurable target SOC |
| Discharging | Discharge at rated power until threshold, then switch to Self-Use |

### Back Up Mode (Write: 2)
Like Self-Use but maintains elevated battery for emergencies. Default min SOC: 30%, target charge: 50%. Grid charging enabled. Battery only discharges during grid outage.
**Priority:** Solar > Battery (maintained high) > Grid.

### Feed-In Priority (Write: 3)
**Energy routing:** PV → House Load → Grid (export) → Battery (charge). Battery does NOT discharge outside configured discharge windows.
**Priority:** Solar > Grid Export > Battery.
Default minimum SOC: 10%.

---

## Manual Mode (Holding Register 0x20)

Used for forced charge/discharge. Also serves as Battery Min Capacity on Gen2-3.

| Value | Sub-mode | Behavior |
|---|---|---|
| 0 | Stop Charge/Discharge | Battery inactive. PV still feeds loads. |
| 1 | Force Charge | Battery charges from grid + PV, subject to max current limits. |
| 2 | Force Discharge | Battery discharges toward grid, limited by export control. |

---

## Time Axis Controls

### Forced Charging Period
Registers 0x26 (start) and 0x27 (end). Default 00:00-00:00 (disabled). Highest priority override — charges battery to target SOC using grid + PV regardless of use mode.

### Allowed Discharging Period
Default 00:00-23:59 (full day enabled). Permits (does not force) battery discharge. Two configurable periods.

### Time Encoding
`time_value = (minutes * 256) + hours`
Decode: `hours = value % 256`, `minutes = value / 256`
Example: 14:30 → 30×256 + 14 = 7694

---

## EEPROM Wear Warning

Use mode writes (register 0x1F) are stored in EEPROM. Estimated ~100,000 write cycles. Limit to ~7 changes/day. For dynamic automation (adjusting power every few minutes), use VPP modes instead — they are NOT stored in EEPROM.

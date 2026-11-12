# Solax Inverter Use Modes

Use modes (working modes) define the inverter's energy routing strategy. They are stored in EEPROM — limit writes to ~7 per day to preserve lifespan (~100,000 write cycles). For frequent automation, use VPP modes instead.

---

## Inverter Status States (Input Register 0x09 / RunMode)

State values differ by generation. Both read from Input Register 0x09 (FC04).

### Gen2 / Gen3

| Value | State | Operational Meaning |
|---|---|---|
| 0 | **Wait Mode** | Waiting for proper conditions (grid voltage, PV voltage). No power conversion. |
| 1 | **Check Mode** | Verifying grid parameters and system health before Normal. |
| 2 | **Normal Mode** | Active grid-tied operation per the selected use mode. |
| 3 | **Off Mode** | Inverter powered off (not a fault). |
| 4 | **Permanent Fault** | Non-recoverable hardware fault. Requires service. |
| 5 | **Update Mode** | Firmware update in progress. Do NOT power off. |
| 6 | **EPS Check Mode** | Verifying conditions before entering EPS/off-grid mode. |
| 7 | **EPS Mode** | Off-grid operation. PV + Battery supply EPS loads. |
| 8 | **Self-Test** | Internal diagnostics. |
| 9 | **Idle Mode** | On but not converting. Battery at min SOC, no PV, no charge window. |
| 10 | **Standby** | Standby state. |

Normal cycle: Wait → Check → Normal. At night: Normal → Idle. During outage: EPS Check → EPS.

### Gen4+

| Value | State | Operational Meaning |
|---|---|---|
| 0 | **Wait Mode** | As Gen2/3. |
| 1 | **Check Mode** | As Gen2/3. |
| 2 | **Normal Mode** | As Gen2/3. |
| 3 | **Fault Mode** | Recoverable fault. May auto-clear. |
| 4 | **Permanent Fault** | As Gen2/3. |
| 5 | **Update Mode** | As Gen2/3. |
| 6 | **Off-Grid Waiting** | Verifying conditions before entering off-grid mode. |
| 7 | **Off-Grid** | Battery + PV supply off-grid loads. |
| 8 | **Self-Test** | As Gen2/3. |
| 9 | **Idle Mode** | As Gen2/3. |
| 10 | **Standby** | Standby state. |
| 20–24 | **TOU Sub-states** | Active during Force Time Use sub-mode transitions. |

---

## Use Mode Register Mapping

**Use mode values differ between Gen2/3 and Gen4+**, applied to both the write register (0x1F) and the read-back register (0x8B) consistently per generation. The previous "read vs. write asymmetry" framing was incorrect — the asymmetry is a generational change.

### Gen2 / Gen3 (registers 0x1F and 0x8B both use this mapping)

| Value | Mode |
|---|---|
| 0 | Self Use |
| 1 | Force Time Use |
| 2 | Back Up |
| 3 | Feed-In Priority |

### Gen4+ (registers 0x1F and 0x8B both use this mapping)

| Value | Mode |
|---|---|
| 0 | Self Use |
| 1 | Feed-In Priority |
| 2 | Back Up |
| 3 | Manual Mode |
| 4 | Peak Shaving |
| 5 | Smart Schedule |

A Gen4 inverter in "Feed-In Priority" will show `1` when read back at 0x8B — consistent with writing `1` to 0x1F. Force Time Use (0x1F = 1 on Gen3) does not exist on Gen4+; its functionality is replaced by Smart Schedule (0x1F = 5).

---

## Use Mode Descriptions

### Self Use (0 on both Gen)
**Energy routing:** PV → House Load → Battery (charge) → Grid (export). When PV insufficient: Battery (discharge if above min SOC) → Grid (import).
**Priority:** Solar > Battery > Grid.
Default minimum SOC: 10%.

### Feed-In Priority (Gen2/3 write=3; Gen4+ write=1)
**Energy routing:** PV → House Load → Grid (export) → Battery (charge). Battery does NOT discharge outside configured discharge windows.
**Priority:** Solar > Grid Export > Battery.
Default minimum SOC: 10%.

### Back Up Mode (2 on both Gen)
Like Self-Use but maintains elevated battery for emergencies. Default min SOC: 30%, target charge: 50%. Grid charging enabled. Battery only discharges during grid outage.
**Priority:** Solar > Battery (maintained high) > Grid.

### Force Time Use / TOU (Gen2/3 only, write=1)
Time-of-use scheduling. Up to 10 date intervals, each with a configurable sub-mode. Supports weekday/weekend schedules.

**TOU Sub-modes:**
| Sub-mode | Behavior |
|---|---|
| Self-Use | Standard self-consumption within this period |
| Battery Hold | Battery inactive except when below min SOC |
| Peak-Shaving | Battery discharges only when load exceeds peak threshold |
| Charging | Aggressive battery charging; optional grid charging; configurable target SOC |
| Discharging | Discharge at rated power until threshold, then switch to Self-Use |

### Manual Mode (Gen4+, write=3)
Forced charge/discharge via register 0x20. See Manual Mode section below.

### Peak Shaving (Gen4+, write=4)
Battery discharges automatically when grid import would exceed the discharge limit threshold. Configure via registers 0xEE–0xF3. See `references/grid.md`.

### Smart Schedule (Gen4+, write=5)
Successor to Force Time Use on Gen4. Time-of-use scheduling with configurable slots.

---

## Manual Mode (Holding Register 0x20)

On Gen2/3, register 0x20 also acts as Battery Min Capacity (0–100%). On Gen4+, it's a dedicated manual-control enum:

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

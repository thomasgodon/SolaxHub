# Solax Grid Interaction

---

## Feed-In Power (Input Register 0x46-0x47)

Signed 32-bit value (S32) from meter or CT:
- **Positive** = exporting power to grid
- **Negative** = importing power from grid

---

## Power Flow Concepts

- **House Load** = Inverter Power − Feed-In Power (derived, not a direct register)
- **Energy Balance:** PV Power ≈ House Load + Battery Charge + Grid Export
- Battery power sign: Positive = charging, Negative = discharging
- Feed-in power sign: Positive = exporting, Negative = importing

---

## Export Control

| Register | Name | Range | Description |
|---|---|---|---|
| 0x42 | Export Control User Limit | 0-60,000 W | Max power inverter will export. Set to 0 for zero-export. |

Scale: 1 W/unit on some models, /10 on others — check model documentation.

---

## Zero-Export (3 Mechanisms)

### 1. Built-in Export Control (Register 0x42)
Requires meter or CT. Inverter detects consumption and limits output to match load. Set register 0x42 = 0 to prevent any export.

### 2. Power Bias (pgrid_bias)
Adds a margin that makes the inverter slightly prefer grid import over export. Configurable per model (e.g., 40W for X1-MINI G4, up to 10% of rated for X3-MEGA). Prevents micro-exports due to measurement latency.

### 3. Per-Phase Control (3-phase only)
Uses the phase with the smallest load as the reference, preventing any single phase from exporting even when the other phases have surplus.

---

## Peak Shaving (Gen4+)

| Register | Name | Description |
|---|---|---|
| 0xEE | Peak Shaving Discharge Limit 1 | W — discharge to prevent grid import above this |
| 0xEF | Peak Shaving Discharge Limit 2 | W |
| 0xF1 | Peak Shaving Charge Limit | W — charge limit during off-peak |
| 0xF2 | Peak Shaving Max SOC | % — max SOC for peak shaving reserve |
| 0xF3 | Peak Shaving Reserved SOC | % — SOC kept exclusively for peak events |

Peak shaving only available on Gen4+. Battery discharges automatically when grid import would exceed the discharge limit threshold.

---

## Main Breaker / Grid Import Limit

| Register | Name | Description |
|---|---|---|
| 0x71 | Main Breaker Current Limit | A — total grid import cap (Gen4-5) |

---

## Generator Integration (Gen4+)

| Register | Name | Description |
|---|---|---|
| 0xE4 | Generator Switch On SOC | % — start generator when battery below this |
| 0xE5 | Generator Switch Off SOC | % — stop generator when battery above this |
| 0xE6 | Generator Max Run Time | min |
| 0xE7 | Generator Min Rest Time | min |

---

## Sign Convention Summary

| Measurement | Positive | Negative |
|---|---|---|
| Feed-in power (0x46-0x47) | Exporting to grid | Importing from grid |
| Battery power (0x16) | Charging | Discharging |
| Battery current (0x15) | Charging | Discharging |
| VPP GridWTarget | Import from grid | Export to grid |
| VPP BatWTarget | Battery discharge | Battery charge |

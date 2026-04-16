# Solax Battery Management

---

## Key Monitoring Registers (Input, FC04)

| Register | Name | Type | Unit | Scale | Notes |
|---|---|---|---|---|---|
| 0x1C | Battery SOC | U16 | % | 1 | 0% may be communication error — verify with voltage |
| 0x16 | Battery Power | S16 | W | 1 | Positive=charging, negative=discharging |
| 0x14 | Battery Voltage | S16 | V | /100 | |
| 0x15 | Battery Current | S16 | A | /100 | Positive=charging |
| 0x18 | Battery Temperature (Charger) | S16 | °C | 1 | |
| 0x55 | Battery Temperature | U16 | °C | /10 | |
| 0x23 | Battery State of Health | U16 | % | 1 | |
| 0x1F | BMS Warning Code | U16 | code | 1 | |
| 0x1D-0x1E | Battery Energy Discharged Total | U32 | kWh | /10 | |
| 0x20-0x21 | Battery Energy Charged Total | U32 | kWh | /10 | |

---

## Key Configuration Registers (Holding, FC03/FC06)

| Register | Name | Range | Unit | Scale | Gen |
|---|---|---|---|---|---|
| 0x24 | Battery Charge Max Current | 0-250 | A | /10 (Gen3), /100 (Gen4+) | All |
| 0x25 | Battery Discharge Max Current | 0-250 | A | /10 (Gen3), /100 (Gen4+) | All |
| 0x20 | Battery Min Capacity (Gen2-3) | 10-100 | % | 1 | Gen2-3 |
| 0x61 | Self-Use Discharge Min SOC | 10-100 | % | 1 | Gen4+ |
| 0x65 | Feed-In Discharge Min SOC | — | % | 1 | Gen4+ |
| 0x67 | Backup Discharge Min SOC | — | % | 1 | Gen4+ |
| 0xE0 | Battery Charge Upper SOC | 0-100 | % | 1 | Gen4+ |
| 0x63 | Self-Use Nightcharge Upper SOC | — | % | 1 | Gen4+ |
| 0x64 | Feed-In Nightcharge Upper SOC | — | % | 1 | Gen4+ |
| 0x66 | Backup Nightcharge Upper SOC | — | % | 1 | Gen4+ |
| 0xA4 | Max Charge Capacity | 0-100 | % | 1 | All |
| 0x56 | Battery Awaken | — | — | 1 | Gen4+ |
| 0xC5 | Self-Use Backup SOC | — | % | 1 | Gen4-5 |

---

## Scaling Factor Change (Gen3 vs Gen4+)

Battery charge/discharge current registers (0x24, 0x25) changed scaling between generations:
- **Gen2-3:** Raw value / 10 = Amps (0.1A resolution)
- **Gen4+:** Raw value / 100 = Amps (0.01A resolution)

Always check the generation before interpreting current values. A raw value of 100 means 10.0A on Gen3 or 1.00A on Gen4+.

---

## BMS Protection

The BMS communicates with the inverter via **CAN bus** and calculates real-time max charge/discharge current.

- User-configurable limits (0x24/0x25) can be further restricted by BMS in real-time based on cell voltages, temperature, and SOH
- When the lowest cell voltage drops below the cutoff threshold (CLOW), discharge ramps to 0.02C then 0A; SOC resets to 3%
- If any cell reaches absolute minimum (CMIN), BMS signals Error 2; SOC resets to 1%
- When SOC reaches the configured minimum discharge threshold, inverter enters Idle Mode (status 9)

---

## Battery Wake

Register 0x56 (Gen4+): Write to wake a sleeping battery. Required if battery entered deep sleep due to prolonged idle or low SOC.

---

## SOC Validation

A reading of 0% SOC may indicate:
1. A communication error (most common)
2. Genuinely empty battery

Before acting on 0% SOC, verify battery voltage is reasonable. If voltage is valid (e.g., above 40V for a 48V system), treat 0% SOC as an error and use the last known value.

---

## Lock/Unlock Requirement

All write operations to battery configuration registers require prior unlock. Write password to holding register 0x0000:
- `2014` = basic unlock
- `6868` = advanced unlock (required for VPP register access)

Read lock state from input register 0x54 (0=Locked, 1=Unlocked, 2=Unlocked Advanced).

Lock state may auto-revert after timeout or inverter reboot. Always re-check before writing.

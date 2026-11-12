# Solax Register Quick Reference

Dense lookup reference for Solax Modbus register addresses, data types, and conversion patterns.

For run-mode state values → `references/use-modes.md`
For use-mode (working-mode) values → `references/use-modes.md`
For lock/unlock passwords and lock-state codes → SKILL.md > Lock/Unlock Quick Reference
For serial-number prefix → inverter type → `references/products.md`
For protocol document versions → `references/products.md`
For data type decoding (U16, S16, U32, strings) → `references/modbus.md`
For U32 overflow / SOC=0 error patterns → `references/modbus.md` / `references/battery.md`
For DataHub register formula → `references/connectivity.md`

---

## Input Registers — Hybrid X1/X3 G4 (FC04)

| Hex | Dec | Name | Type | Unit | Scale | Notes |
|-----|-----|------|------|------|-------|-------|
| 0x00 | 0 | Grid Voltage (Phase R) | U16 | V | /10 | |
| 0x01 | 1 | Grid Current (Phase R) | S16 | A | /10 | |
| 0x02 | 2 | Inverter Power / AC Power | S16 | W | 1 | |
| 0x03 | 3 | PV1 Voltage | U16 | V | /10 | |
| 0x04 | 4 | PV2 Voltage | U16 | V | /10 | |
| 0x05 | 5 | PV1 Current | U16 | A | /10 | |
| 0x06 | 6 | PV2 Current | U16 | A | /10 | |
| 0x07 | 7 | Grid Frequency | U16 | Hz | /100 | |
| 0x08 | 8 | Inverter Temperature | S16 | C | 1 | |
| 0x09 | 9 | Run Mode | U16 | enum | 1 | See use-modes.md Run Mode table |
| 0x0A | 10 | PV1 Power (DC1) | U16 | W | 1 | |
| 0x0B | 11 | PV2 Power (DC2) | U16 | W | 1 | |
| 0x14 | 20 | Battery Voltage | S16 | V | /100 | |
| 0x15 | 21 | Battery Current | S16 | A | /10 (Gen3+), /100 (Gen2) | Positive=charging |
| 0x16 | 22 | Battery Power | S16 | W | 1 | Positive=charging, Negative=discharging |
| 0x17 | 23 | Charger Board Temperature | S16 | C | 1 | |
| 0x18 | 24 | Charger Battery Temperature | S16 | C | 1 | |
| 0x19 | 25 | Charger Boost Temperature | S16 | C | 1 | |
| 0x1C | 28 | Battery SOC | U16 | % | 1 | 0-100 |
| 0x1D-0x1E | 29-30 | Battery Energy Discharged Total | U32 | kWh | /10 | |
| 0x1F | 31 | BMS Warning Code | U16 | code | 1 | |
| 0x20-0x21 | 32-33 | Battery Energy Charged Total | U32 | kWh | /10 | |
| 0x23 | 35 | Battery State of Health | U16 | % | 1 | |
| 0x40-0x41 | 64-65 | Inverter Fault Code | U32 | code | 1 | |
| 0x42 | 66 | Charger Fault Code | U16 | code | 1 | |
| 0x43 | 67 | Manager Fault Code | U16 | code | 1 | |
| 0x46-0x47 | 70-71 | Feed-In Power (Meter/CT) | S32 | W | 1 | Positive=export, Negative=import |
| 0x48-0x49 | 72-73 | Feed-In Energy Total | U32 | kWh | /100 | |
| 0x4A-0x4B | 74-75 | Consumed Energy Total | U32 | kWh | /100 | |
| 0x4C | 76 | EPS Voltage | U16 | V | /10 | |
| 0x4D | 77 | EPS Current | U16 | A | /10 | |
| 0x4E | 78 | EPS VA Power | U16 | VA | 1 | |
| 0x4F | 79 | EPS Frequency | U16 | Hz | /100 | |
| 0x50 | 80 | Energy Today (Yield) | U16 | kWh | /10 | |
| 0x52-0x53 | 82-83 | Energy Total (Yield) | U32 | MWh | /1000 | |
| 0x54 | 84 | Lock State | U16 | enum | 1 | 0=Locked, 1=Unlocked, 2=Advanced |
| 0x55 | 85 | Battery Temperature | U16 | C | /10 | |
| 0x6B | 107 | Grid Current Phase R | S16 | A | /10 | X3 only |
| 0x6C | 108 | Grid Power Phase R | S16 | W | 1 | X3 only |
| 0x6F | 111 | Grid Current Phase S | S16 | A | /10 | X3 only |
| 0x70 | 112 | Grid Power Phase S | S16 | W | 1 | X3 only |
| 0x73 | 115 | Grid Current Phase T | S16 | A | /10 | X3 only |
| 0x74 | 116 | Grid Power Phase T | S16 | W | 1 | X3 only |
| 0x8B | 139 | Solar Charger Use Mode | U16 | enum | 1 | Read current use mode (Gen-specific mapping — see use-modes.md) |
| 0x94-0x95 | 148-149 | Solar Energy Total | U32 | kWh | /10 | |
| 0x96 | 150 | Solar Energy Today | U16 | kWh | /10 | |
| 0x98 | 152 | Feed-In Energy Today | U16 | kWh | /100 | |
| 0x9A | 154 | Consumed Energy Today | U16 | kWh | /100 | |
| 0xAF | 175 | Registration Code | STR | ASCII | 5 words | 10 characters |
| 0xBE | 190 | Radiator Temperature | S16 | C | 1 | |
| 0x100 | 256 | Modbus Power Control Status | U16 | enum | 1 | Current VPP mode active |

---

## Input Registers — Non-Hybrid X1 Boost/Mini/X3 MIC (FC04, 0x0400-block)

Non-hybrid (MIC/Boost) inverters expose a separate, smaller register set in the 0x0400–0x04xx range. These are not "hybrid registers + 0x400" — they are a distinct set for the non-hybrid product line.

| Hex | Dec | Name | Type | Unit | Scale |
|-----|-----|------|------|------|-------|
| 0x400 | 1024 | PV1 Input Voltage | U16 | V | /10 |
| 0x401 | 1025 | PV2 Input Voltage | U16 | V | /10 |
| 0x402 | 1026 | PV1 Input Current | U16 | A | /10 |
| 0x403 | 1027 | PV2 Input Current | U16 | A | /10 |
| 0x404 | 1028 | Grid Voltage | U16 | V | /10 |
| 0x407 | 1031 | Grid Frequency | U16 | Hz | /100 |
| 0x40A | 1034 | Output Current | U16 | A | /10 |
| 0x40D | 1037 | Temperature | U16 | C | 1 |
| 0x40E | 1038 | Inverter Power | U16 | W | 1 |
| 0x415 | 1045 | Run Mode | U16 | enum | 1 |
| 0x414 | 1044 | Power DC1 | U16 | W | 1 |
| 0x423 | 1059 | Production Total | U16 | kWh | /10 |
| 0x425 | 1061 | Production Today | U16 | kWh | /10 |

---

## Holding Registers — Configuration & Control (FC03/FC06/FC16)

| Hex | Dec | Name | Type | Unit | Scale | Range | Gen |
|-----|-----|------|------|------|-------|-------|-----|
| 0x00 | 0 | Lock Password | U16 | - | 1 | 0/2014/6868 | All |
| 0x00-0x05 | 0-5 | RTC Sync (s,m,h,d,M,Y) | U16x6 | - | 1 | Date/time | Gen3+ |
| 0x1C | 28 | System On/Off | U16 | - | 1 | 0=Off, 1=On | All |
| 0x1F | 31 | Charger Use Mode | U16 | enum | 1 | Gen-specific — see use-modes.md | All |
| 0x20 | 32 | Manual Mode / Battery Min Cap | S16/U16 | % | 1 | 0-100 or 0-2 | Varies |
| 0x24 | 36 | Battery Charge Max Current | U16 | A | /10 (Gen3+), /100 (Gen2) | 0-250 | All |
| 0x25 | 37 | Battery Discharge Max Current | U16 | A | /10 (Gen3+), /100 (Gen2) | 0-250 | All |
| 0x26 | 38 | Force Time Start | U16 | time | min*256+hr | - | All |
| 0x27 | 39 | Force Time End | U16 | time | min*256+hr | - | All |
| 0x42 | 66 | Export Control User Limit | U16 | W | 1 or /10 | 0-60,000 | All |
| 0x56 | 86 | Battery Awaken | U16 | - | 1 | - | Gen4+ |
| 0x61 | 97 | Self-Use Discharge Min SOC | S16 | % | 1 | 10-100 | Gen4+ |
| 0x63 | 99 | Self-Use Nightcharge Upper SOC | S16 | % | 1 | - | Gen4+ |
| 0x64 | 100 | Feed-In Nightcharge Upper SOC | S16 | % | 1 | - | Gen4+ |
| 0x65 | 101 | Feed-In Discharge Min SOC | S16 | % | 1 | - | Gen4+ |
| 0x66 | 102 | Backup Nightcharge Upper SOC | S16 | % | 1 | - | Gen4+ |
| 0x67 | 103 | Backup Discharge Min SOC | S16 | % | 1 | - | Gen4+ |
| 0x71 | 113 | Main Breaker Current Limit | S16 | A | 1 | - | Gen4-5 |
| 0xA4 | 164 | Max Charge Capacity | U16 | % | 1 | 0-100 | All |
| 0xC5 | 197 | Self-Use Backup SOC | S16 | % | 1 | - | Gen4-5 |
| 0xE0 | 224 | Battery Charge Upper SOC | S16 | % | 1 | 0-100 | Gen4+ |
| 0xE4 | 228 | Generator Switch On SOC | S16 | % | 1 | - | Gen4+ |
| 0xE5 | 229 | Generator Switch Off SOC | S16 | % | 1 | - | Gen4+ |
| 0xE6 | 230 | Generator Max Run Time | S16 | min | 1 | - | Gen4+ |
| 0xE7 | 231 | Generator Min Rest Time | S16 | min | 1 | - | Gen4+ |
| 0xEE | 238 | Peak Shaving Discharge Limit 1 | S16 | W | 1 | - | Gen4-5 |
| 0xEF | 239 | Peak Shaving Discharge Limit 2 | S16 | W | 1 | - | Gen4+ |
| 0xF1 | 241 | Peak Shaving Charge Limit | S16 | W | 1 | - | Gen4+ |
| 0xF2 | 242 | Peak Shaving Max SOC | S16 | % | 1 | - | Gen4+ |
| 0xF3 | 243 | Peak Shaving Reserved SOC | S16 | % | 1 | - | Gen4+ |

---

## Holding Registers — Serial Number (FC03)

| Hex | Dec | Name | Type | Words | Description |
|-----|-----|------|------|-------|-------------|
| 0x00 | 0 | Series Number | STR | 7 | 14 ASCII characters. First 3 chars = inverter type prefix. |

Note: Holding register 0x00 is dual-purpose — serial number (when read) and lock password (when written).

---

## VPP Registers — Legacy (0x7C block, FC16)

| Hex | Dec | Name | Type | Unit |
|-----|-----|------|------|------|
| 0x7C | 124 | VPP Mode Number | U16 | 0=Off, 1-7=Mode |
| 0x7E-0x7F | 126-127 | Active Power | S32 | W |
| 0x80-0x81 | 128-129 | Reactive Power | S32 | VAR |
| 0x82 | 130 | Duration | U16 | sec |
| 0x83 | 131 | Target SOC | U16 | % |
| 0x84-0x85 | 132-133 | Target Energy | S32 | Wh |
| 0x86-0x87 | 134-135 | Charge/Discharge Power | S32 | W (swapped word order!) |
| 0x88 | 136 | Timeout | U16 | sec |
| 0x89-0x8A | 137-138 | Push Mode Power | S32 | W (swapped word order!) |

## VPP Registers — New API (0xA0 block, FC16, Gen4+ only)

| Hex | Dec | Name | Type | Unit |
|-----|-----|------|------|------|
| 0xA0 | 160 | VPP Mode Number | U16 | 0=Off, 1-9=Mode |
| 0xA1 | 161 | TargetSetType | U16 | 1=Set (replace), 2=Update (cumulative) |
| 0xA2-0xA3 | 162-163 | PV Power Limit / GridWTarget | U32/S32 | W (swapped word order!) |
| 0xA4-0xA5 | 164-165 | Push Mode Power / GridVarTarget | S32 | W/VAR (swapped word order!) |
| 0xA6 | 166 | Duration / Target SOC | U16 | sec or % |
| 0xA7 | 167 | Timeout | U16 | sec |

For full VPP mode descriptions and parameter semantics → `references/vpp.md`

---

## Scaling Factor Reference

| Measurement | Scale | Example |
|---|---|---|
| Voltage (grid, PV) | /10 | Raw 2301 = 230.1V |
| Voltage (battery) | /100 | Raw 5120 = 51.20V |
| Current (grid, PV) | /10 | Raw 45 = 4.5A |
| Current (battery Gen2) | /100 | Raw 100 = 1.00A |
| Current (battery Gen3+) | /10 | Raw 100 = 10.0A |
| Frequency | /100 | Raw 5000 = 50.00Hz |
| Power (W) | 1 | Raw 3500 = 3500W |
| Energy (kWh) daily | /10 | Raw 125 = 12.5kWh |
| Energy (kWh) total | /10 or /100 | Varies by register |
| Energy (MWh) yield total | /1000 | Raw 15000 = 15.000MWh |
| Temperature | 1 or /10 | Varies by register |
| SOC | 1 | Raw 85 = 85% |

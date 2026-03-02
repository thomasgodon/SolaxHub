# Solax Register Quick Reference

Dense lookup reference for Solax Modbus register addresses, data types, and conversion patterns.

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
| 0x09 | 9 | Run Mode | U16 | enum | 1 | See Run Mode table |
| 0x0A | 10 | PV1 Power (DC1) | U16 | W | 1 | |
| 0x0B | 11 | PV2 Power (DC2) | U16 | W | 1 | |
| 0x14 | 20 | Battery Voltage | S16 | V | /100 | |
| 0x15 | 21 | Battery Current | S16 | A | /100 | Positive=charging |
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
| 0x8B | 139 | Solar Charger Use Mode | U16 | enum | 1 | Read current use mode |
| 0x94-0x95 | 148-149 | Solar Energy Total | U32 | kWh | /10 | |
| 0x96 | 150 | Solar Energy Today | U16 | kWh | /10 | |
| 0x98 | 152 | Feed-In Energy Today | U16 | kWh | /100 | |
| 0x9A | 154 | Consumed Energy Today | U16 | kWh | /100 | |
| 0xAF | 175 | Registration Code | STR | ASCII | 5 words | 10 characters |
| 0xBE | 190 | Radiator Temperature | S16 | C | 1 | |
| 0x100 | 256 | Modbus Power Control Status | U16 | enum | 1 | Current VPP mode active |

---

## Input Registers — Non-Hybrid X1 Boost/Mini/X3 MIC (FC04, base 0x0400)

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
| 0x40F | 1039 | Run Mode | U16 | enum | 1 |
| 0x414 | 1044 | Power DC1 | U16 | W | 1 |
| 0x415 | 1045 | Power DC2 | U16 | W | 1 |
| 0x423 | 1059 | Production Total | U16 | kWh | /10 |
| 0x425 | 1061 | Production Today | U16 | kWh | /10 |

---

## Holding Registers — Configuration & Control (FC03/FC06/FC16)

| Hex | Dec | Name | Type | Unit | Scale | Range | Gen |
|-----|-----|------|------|------|-------|-------|-----|
| 0x00 | 0 | Lock Password | U16 | - | 1 | 0/2014/6868 | All |
| 0x00-0x05 | 0-5 | RTC Sync (s,m,h,d,M,Y) | U16x6 | - | 1 | Date/time | Gen3+ |
| 0x1C | 28 | System On/Off | U16 | - | 1 | 0=Off, 1=On | All |
| 0x1F | 31 | Charger Use Mode | U16 | enum | 1 | 0-3 | All |
| 0x20 | 32 | Manual Mode / Battery Min Cap | S16/U16 | % | 1 | 0-100 or 0-2 | Varies |
| 0x24 | 36 | Battery Charge Max Current | U16 | A | /10 or /100 | 0-250 | All |
| 0x25 | 37 | Battery Discharge Max Current | U16 | A | /10 or /100 | 0-250 | All |
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

Note: Holding register 0x00 is dual-purpose — it's both the serial number (when read) and the lock password (when written).

---

## VPP Registers — Mode 1-7 (0x7C block, FC16)

| Hex | Dec | Name | Type | Unit |
|-----|-----|------|------|------|
| 0x7C | 124 | Power Control Trigger | U16 | 0=Off, 1=On |
| 0x7E-0x7F | 126-127 | Active Power | S32 | W |
| 0x80-0x81 | 128-129 | Reactive Power | S32 | VAR |
| 0x82 | 130 | Duration | U16 | sec |
| 0x83 | 131 | Target SOC | U16 | % |
| 0x84-0x85 | 132-133 | Target Energy | S32 | Wh |
| 0x86-0x87 | 134-135 | Charge/Discharge Power | S32 | W (swapped word order!) |
| 0x88 | 136 | Timeout | U16 | sec |
| 0x89-0x8A | 137-138 | Push Mode Power | S32 | W (swapped word order!) |

## VPP Registers — Mode 8-9 (0xA0 block, FC16, Gen4+ only)

| Hex | Dec | Name | Type | Unit |
|-----|-----|------|------|------|
| 0xA0 | 160 | Mode 8/9 Trigger | U16 | 0=Off, 8=Duration, 9=SOC |
| 0xA2-0xA3 | 162-163 | PV Power Limit | U32 | W |
| 0xA4-0xA5 | 164-165 | Push Mode Power | S32 | W |
| 0xA6 | 166 | Duration / Target SOC | U16 | sec or % |
| 0xA7 | 167 | Timeout | U16 | sec |

---

## Data Type Conversion Patterns

### U16 (unsigned 16-bit)
Read 1 register. If host is little-endian, swap bytes: `BitConverter.ToUInt16([byte[1], byte[0]])`

### S16 (signed 16-bit)
Read 1 register. Same byte swap: `BitConverter.ToInt16([byte[1], byte[0]])`

### U32 / S32 (32-bit, 2 registers)
Standard word order (big-endian): `(register[1] << 16) | (register[0] & 0xFFFF)`
Swapped word order: `(register[0] << 16) | (register[1] & 0xFFFF)`

Known swapped registers: 0x86-0x87, 0x89-0x8A

### Strings (multi-word)
Read N registers, decode as ASCII: `Encoding.ASCII.GetString(bytes)`
Serial number: 7 words = 14 bytes. Registration code: 5 words = 10 bytes.

### Packed Time
`time_value = (minutes * 256) + hours`
Decode: `hours = value % 256`, `minutes = value / 256`

### U8 High/Low byte
Some values use only one byte of a 16-bit register:
- High byte: `(register >> 8) & 0xFF`
- Low byte: `register & 0xFF`

---

## Scaling Factor Reference

| Measurement | Scale | Example |
|---|---|---|
| Voltage (grid, PV) | /10 | Raw 2301 = 230.1V |
| Voltage (battery) | /100 | Raw 5120 = 51.20V |
| Current (grid, PV) | /10 | Raw 45 = 4.5A |
| Current (battery Gen3) | /10 | Raw 100 = 10.0A |
| Current (battery Gen4+) | /100 | Raw 1000 = 10.00A |
| Frequency | /100 | Raw 5000 = 50.00Hz |
| Power (W) | 1 | Raw 3500 = 3500W |
| Energy (kWh) daily | /10 | Raw 125 = 12.5kWh |
| Energy (kWh) total | /10 or /100 | Varies by register |
| Energy (MWh) yield total | /1000 | Raw 15000 = 15.000MWh |
| Temperature | 1 or /10 | Varies by register |
| SOC | 1 | Raw 85 = 85% |

---

## Run Mode Values

| Value | State |
|---|---|
| 0 | Wait Mode |
| 1 | Check Mode |
| 2 | Normal Mode |
| 3 | Fault Mode |
| 4 | Permanent Fault |
| 5 | Update Mode |
| 6 | EPS Check Mode |
| 7 | EPS Mode |
| 8 | Self-Test |
| 9 | Idle Mode |

---

## Use Mode Values

### Read (Input Register 0x8B)

| Value | Mode |
|---|---|
| 0 | Self Use |
| 1 | Feed-In Priority |
| 2 | Back Up |
| 3 | Force Time Use |

### Write (Holding Register 0x1F)

| Value | Mode |
|---|---|
| 0 | Self Use |
| 1 | Force Time Use |
| 2 | Back Up |
| 3 | Feed-In Priority |

**Important:** The value-to-mode mapping differs between read and write registers! Feed-In Priority is 1 when read but 3 when written.

---

## Lock State Mapping

### Reading (Input Register 0x54)

| Value | State |
|---|---|
| 0 | Locked |
| 1 | Unlocked |
| 2 | Unlocked Advanced |

### Writing (Holding Register 0x00)

| Password | Access Level |
|---|---|
| 0 | Lock |
| 2014 | Unlock (basic) |
| 6868 | Unlock Advanced (full VPP access) |

---

## Serial Number Prefix -> Inverter Type

| Prefix | Model | Enum |
|---|---|---|
| H43, H450, H460, H475 | X1-Hybrid G4 | 15 |
| H34A, H34B, H34T | X3-Hybrid G4 | 14 |
| H1E, HCC, HUE, XRE | X1-Hybrid Gen3 | 2 |
| H3DE, H3PE, H3UE | X3-Hybrid Gen3 | 5 |
| (various) | X1-LX | 1 |
| (various) | X1-Boost/Air/Mini | 4 |
| (various) | X3-MIC Pro | 7 |
| (various) | X1-Smart | 8 |
| (various) | X1-AC | 9 |
| (various) | A1-Hybrid | 10 |
| (various) | A1-Fit | 11 |
| (various) | A1-Grid | 12 |
| (various) | J1-ESS | 13 |
| (various) | X3-MIC Pro G2 | 16 |
| (various) | X1-SPT | 17 |
| (various) | X1-Boost Mini G4 | 18 |
| (various) | A1-Hybrid G2 | 19 |
| (various) | A1-AC G2 | 20 |
| (various) | A1-Smart G2 | 21 |
| (various) | X3-FTH | 22 |
| (various) | X3-MEGA G2 | 23 |

---

## Modbus Protocol Document Versions

| Version | Title | Date | Coverage |
|---|---|---|---|
| V1.8 | External Communication Protocol X1 | Early | X1 non-hybrid, proprietary (not standard Modbus) |
| V3.14 | External Communication Protocol X1&X3 Hybrid G4 ModbusRTU | ~2022 | Gen4 RTU |
| V3.21 | Hybrid X1&X3-G4 ModbusTCP&RTU | Jun 2022 | Gen3/G4 TCP+RTU, widely referenced |
| V3.24 | Hybrid X1&X3-G4 ModbusTCP&RTU | Mar 2023 | Updated register set |
| V001.00 | Hybrid X1&X3 ESS Modbus RTU | Jun 2025 | Latest, Gen6/VAST support |

PDFs not publicly posted on SolaX downloads page. Obtain from SolaX support or community discussions.

---

## U32 Overflow Detection

Some 32-bit energy sensors can show `0xFFFFFF00` pattern on overflow or communication error. Implementations should detect this and return the last known valid value instead.

## Battery SOC Validation

A SOC reading of 0 may indicate a communication error rather than an empty battery. Treat 0% SOC as potentially invalid and verify with battery voltage.

## DataHub Register Formula

For multi-inverter setups via DataHub 1000:
```
register = 35000 + (port - 1) * 800 + (device_address - 1) * 40 + offset
```
Where port = 1-4, device_address = 1-20, offset = register within the 40-register block per device.

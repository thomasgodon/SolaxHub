# Solax Modbus Protocol

Solax inverters communicate via Modbus TCP (port 502) or Modbus RTU (RS485 serial, 19200 baud, 8N1).

---

## Function Codes

| FC | Name | Usage |
|---|---|---|
| FC03 | Read Holding Registers | Read writable configuration/control registers |
| FC04 | Read Input Registers | Read read-only measurement/status data |
| FC06 | Write Single Register | Write individual 16-bit configuration values |
| FC16 (0x10) | Write Multiple Registers | Write 32-bit values or VPP commands; required by Gen4+ for remote control (NOT stored in EEPROM) |

Some Solax devices only respond to FC16 even for single-register writes. Gen4+ VPP commands specifically require FC16 to avoid EEPROM wear.

---

## Byte Ordering

- **Within a 16-bit register:** Big-endian (MSB first) — standard Modbus convention
- **32-bit values (2 registers):** Most use swapped word order — LSB at lower address, MSB at higher address. Opposite of standard Modbus big-endian.
- **Standard (non-swapped) 32-bit:** `(msb_reg << 16) | (lsb_reg & 0xFFFF)` — used for energy totals
- **Swapped 32-bit:** Same decode formula but register pair is (LSB, MSB) — used for VPP registers and some power registers (0x86-0x87, 0x89-0x8A)
- **Strings:** ASCII encoded, multiple registers

---

## Connection Setup

| Parameter | TCP | RTU |
|---|---|---|
| Port / Interface | 502 | RS485 serial |
| Baud rate | N/A | 19200 (some models 9600) |
| Data/Parity/Stop | N/A | 8N1 |
| Default Unit ID | 1 | 1 (configurable) |
| Recommended timeout | 5 seconds | 5 seconds |
| Minimum poll interval | 3-5 seconds | 3-5 seconds |

Access via built-in Ethernet port (Gen3), Pocket WiFi dongle, or external RS485-to-Ethernet adapter (Waveshare recommended over SolaX dongle for reliability).

Default dongle WiFi SSID: `WiFi_Sxxxxxxxxx`, portal at `http://5.8.8.8/` with credentials `admin/admin` or `admin/<dongleSN>`.

---

## Block Reading Strategy

Efficient implementations group consecutive registers into blocks (up to 100 registers per read) to minimize Modbus round-trips. Multi-register entities (32-bit, strings) must not be split across blocks.

A bisect algorithm can identify unsupported registers by recursively splitting blocks on read errors.

**Single-master constraint:** Modbus TCP to the dongle is single-master. Multiple simultaneous connections cause frame collisions. Use a TCP multiplexer if multiple consumers need access.

---

## Register Base Addresses

| Model Type | Register Base |
|---|---|
| Hybrid inverters (X1/X3 Hybrid, all gens) | 0x0000 |
| Non-hybrid string inverters (X1-Boost, X3-MIC, etc.) | 0x0400 |
| DataHub 1000 multi-inverter | 35000 + formula |

DataHub formula: `register = 35000 + (port-1)*800 + (device_addr-1)*40 + offset`

---

## Data Type Decoding

### U16 (unsigned 16-bit)
Read 1 register. Swap bytes if host is little-endian: `BitConverter.ToUInt16([byte[1], byte[0]])`

### S16 (signed 16-bit)
Read 1 register. Same byte swap: `BitConverter.ToInt16([byte[1], byte[0]])`

### U32 / S32 (32-bit, 2 registers)
- Standard word order: `(high_reg << 16) | (low_reg & 0xFFFF)` — high word at lower address
- Swapped word order: `(reg[1] << 16) | (reg[0] & 0xFFFF)` — low word at lower address (used by VPP registers)

### Packed Time
`time_value = (minutes * 256) + hours`
Decode: `hours = value % 256`, `minutes = value / 256`
Example: 14:30 = 30*256 + 14 = 7694

### Strings
Read N registers, decode as ASCII: `Encoding.ASCII.GetString(bytes)`
Serial number: 7 registers = 14 bytes. Registration code: 5 registers = 10 bytes.

---

## Overflow / Error Patterns

- **U32 overflow:** Some 32-bit energy sensors show `0xFFFFFF00` pattern on overflow or communication error. Use last known valid value instead.
- **SOC = 0:** May indicate communication error rather than empty battery. Verify with battery voltage before acting on 0% SOC.

---

## Protocol Variants

| Variant | Notes |
|---|---|
| Modbus TCP (port 502) | Primary for Gen3+ with Ethernet or dongle |
| Modbus RTU (RS485) | For Gen4 without Ethernet, SolarAssistant, direct wiring |
| DataHub | FC04 for reads (max 125 registers), FC06 for writes |
| X1 Mini G4 | 15s minimum poll interval only |
| X1 Mini Gen3 | Firmware 1.37+ required |
| X3-MIC Gen1 | Firmware 1.38+, serial Modbus only, min 5s poll |
| X3-MIC Gen2 | Firmware 1.17+, serial Modbus |

# Solax Product Families

---

## Hybrid (Energy Storage) Inverters — Full Modbus R/W + VPP

| Model | Phase | Generations | VPP Support |
|---|---|---|---|
| X1-Hybrid | Single | Gen2, Gen3, Gen4 | Gen4: Modes 1-12 |
| X3-Hybrid | Three | Gen3, Gen4, Gen4 Pro | Gen4: Modes 1-12 |
| X1-IES | Single | Current | TBD |
| X3-IES | Three | Current | TBD |
| X3-Ultra | Three | Current | TBD |
| X1-SPT | Single | Current | TBD |
| X-ESS G4 | All-in-one | Current | TBD |

---

## AC-Coupled / Retrofit

| Model | Phase | Notes |
|---|---|---|
| X1-AC | Single | Gen3, Gen4; serial Modbus |
| X3-Fit | Three | Serial Modbus |
| A1-Hybrid | Split-phase | US/AU market |
| J1-ESS | Split-phase | US/AU market |

---

## String (PV-Only) — Read-only Modbus, no battery/VPP

| Model | Phase | Notes |
|---|---|---|
| X1-Mini | Single | Gen3 (FW 1.37+), Gen4 (15s polling minimum) |
| X1-Boost | Single | Gen3, Gen4 |
| X1-Air | Single | Gen3 |
| X3-MIC | Three | Gen1 (FW 1.38+), Gen2 (FW 1.17+); serial Modbus, min 5s poll |
| X3-MIC PRO | Three | Gen2 |
| X3-PRO G2 | Three | Current |
| X3-MEGA G2 | Three | Current |
| X3-FORTH | Three | Gen2 |

**Non-hybrid models use a separate register set in the 0x0400-block** — not hybrid registers with an offset. See `references/register-reference.md`.

---

## Generation Differences (Protocol Impact)

| Aspect | Gen2 | Gen3 | Gen4+ |
|---|---|---|---|
| Ethernet | External adapter | Built-in | External adapter or dongle |
| Battery current scale | /100 (0.01A) | /10 (0.1A) | /10 (0.1A) |
| VPP remote control | Not documented | Limited | Full modes 1-9 (+ 11-12 in newer firmware) |
| VPP legacy block | — | 0x7C (modes 0-7) | 0x7C + 0xA0 (all modes) |
| Peak shaving | No | No | Yes (0xEE-0xF3) |
| Generator control | No | No | Yes (0xE4-0xE7) |
| SOC configuration | Single min SOC (0x20) | Single min SOC (0x20) | Per-mode SOC (0x61, 0x65, 0x67) |
| Use mode set | 4 modes | 4 modes | 6 modes (adds Manual, PeakShaving, Smart Schedule) |

---

## Serial Number Prefix → Inverter Type

The inverter type can be determined from the first 3+ characters of the serial number (holding register 0x00, 7 words = 14 ASCII chars).

**Note:** Inverter Type enum values (numbers in the Enum column below) are SolaX-internal identifiers surfaced in some integrations and protocol documents; not all inverters expose this enum directly. Use the serial-number prefix as the primary identification method.

| Prefix | Model | Type Enum |
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

Serial number: 7 registers (FC03) = 14 ASCII characters. Note: register 0x00 is dual-purpose — serial number when read, lock password when written.

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

## Important Model Caveats

- **X1-Mini Gen3/G4**: Uses a proprietary RS485 protocol (frame header AA.55, custom commands) — NOT standard Modbus.
- **Non-hybrid models**: Register set in 0x0400-block, read-only, no VPP.
- **X1-Mini G4**: 15-second minimum polling interval enforced.
- **X3-MIC**: Serial Modbus only, minimum 5-second poll interval.

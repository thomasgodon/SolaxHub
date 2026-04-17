# Solax Connectivity, APIs & Integrations

---

## Pocket WiFi Dongles

| Model | Connectivity | Protocols |
|---|---|---|
| Pocket WiFi V3.0-P | WiFi 2.4GHz | Modbus TCP, IEEE 2030.5, OpenADR |
| Pocket WiFi+LAN | WiFi + Ethernet | Modbus TCP, IEEE 2030.5, OpenADR |
| Pocket WiFi+4GM | WiFi + 4G/LTE | Modbus TCP, IEEE 2030.5 |

IP65 rated, −35 to +60°C. Data refresh: configurable 5-minute or 10-second intervals.

Default SSID: `WiFi_Sxxxxxxxxx`. Portal at `http://5.8.8.8/` (credentials: `admin/admin` or `admin/<dongleSN>`).

**Reliability note:** Dedicated RS485-to-Ethernet adapter (Waveshare recommended) is more reliable than the SolaX dongle for continuous Modbus TCP connections.

---

## Local HTTP API (Legacy — Being Deprecated)

**Endpoint:** `POST http://<INVERTER_IP>/`
**Auth:** `pwd=<POCKET_WIFI_SERIAL_NUMBER>`
**Operations:** `ReadRealTimeData`, `ReadSetData`, `setReg`, `BatteryMinEnergy`, `SolarChargerUseMode`

**Deprecation warning:** Newer dongle firmware removes this API when connected to home WiFi. It remains accessible via the inverter's own SSID (`WiFi_Sxxxxxxxxx`) at `http://5.8.8.8/`. **Modbus TCP on port 502 is the recommended replacement.**

---

## SolaxCloud REST API (V6.1)

| Parameter | Value |
|---|---|
| Endpoint | `GET https://www.solaxcloud.com/proxyApp/proxy/api/getRealtimeInfo.do` |
| Auth | TokenID (free, from SolaxCloud API page) + Registration Number |
| Rate limits | Max 10 requests/minute, 10,000/day |
| Data freshness | Updated every 5 minutes |
| Response | JSON: acpower, yieldtoday, yieldtotal, inverterStatus, feedinpower, soc, batPower, etc. |

---

## DataHub 1000 (Multi-inverter)

For installations with multiple inverters:
- 4x RS485 ports, up to 60 devices (20 per port)
- FC04 for reads (max 125 registers), FC06 for writes
- Register formula: `35000 + (port-1)*800 + (device_addr-1)*40 + offset`
  - port = 1-4, device_addr = 1-20, offset = register offset within device block

---

## Protocol Comparison

| Protocol | Direction | Latency | Auth | Best For |
|---|---|---|---|---|
| Modbus TCP (502) | Local, R/W | Real-time (3-15s) | None | Full local control, battery management |
| Modbus RTU (RS485) | Local, R/W | Real-time | None | Gen4 without Ethernet |
| Local HTTP API | Local, R/W | Real-time | WiFi SN | Legacy monitoring (deprecated) |
| SolaxCloud REST | Cloud, Read | 5 minutes | TokenID | Remote monitoring without local access |
| IEEE 2030.5 | Utility, R/W | Real-time | Cert-based | Utility VPP programs |
| OpenADR | Utility, R/W | Real-time | TLS/Cert | Demand response programs |

---

## Third-Party Integration Ecosystem

### Home Assistant (3 approaches)

1. **Official Core Integration** (`solax`): Local HTTP API, auto-discovery. Depends on deprecated API. ~725 installations.
2. **Community Modbus (HACS)** — RECOMMENDED: [wills106/homeassistant-solax-modbus](https://github.com/wills106/homeassistant-solax-modbus). Full R/W, Gen2-Gen5 support, VPP power control.
3. **Cloud API integrations** (HACS): Multiple community packages using SolaxCloud REST API.

### Other Platforms

- **openHAB:** Official Solax binding (HTTP + Cloud) + community Modbus approach
- **SolarAssistant:** RS485 → MQTT bridge, integrates with Node-RED and HA
- **Node-RED:** Direct Modbus TCP nodes or MQTT from SolarAssistant
- **evcc:** EV charge controller with SolaX Modbus support
- **Tasmota:** SolaX X1 support via serial
- **Python:** [squishykid/solax](https://github.com/squishykid/solax) async library (local HTTP API)

---

## Firmware Architecture

SolaX inverters have two firmware components:
- **ARM firmware** — communication, UI, integration logic
- **DSP firmware** — power conversion, electrical functions

Pocket WiFi dongles have separate firmware (e.g., V3.003.02, V3.004.03).

### Minimum Versions for Modbus

| Model | Minimum Firmware |
|---|---|
| X1 Air/Boost/Mini Gen3 | 1.37+ |
| X3 MIC Gen1 | 1.38+ |
| X3 MIC Gen2 | 1.17+ |
| X1-Mini G4 | Default 15s polling only |
| Pocket WiFi 3.0 | V3.004.03+ for HA Modbus |

### VPP Firmware Requirements

- Full VPP (modes 1-9): DSP v1.52+, ARM v1.50+
- VPP reaction speed: as fast as 0.092 seconds

### Update Methods

1. **Remote via SolaX support** — pushed through cloud (recommended)
2. **Manual USB** — USB stick <32GB FAT32, PV >180V, battery SOC >20%. Update ARM first, wait 1 min, then DSP.
3. **Dongle web interface** — Pocket WiFi 3.0 System tab

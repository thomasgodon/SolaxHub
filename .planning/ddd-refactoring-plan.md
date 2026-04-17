# DDD Clean Architecture Refactoring Plan for SolaxHub

## Context

The current SolaxHub is a working .NET 9 Worker Service but suffers from an **anemic domain model**:
- `SolaxData` is a flat, behavior-free data bag with 23 properties
- 22 individual MediatR query handlers each call infrastructure (Modbus) directly
- No Aggregate Root, no Value Objects, no Repository abstraction
- Domain logic (scaling, byte-order) is scattered across handler files
- All layers (domain, application, infrastructure, host) live in one project

The goal is to refactor this into a **school example of DDD Clean Architecture** with compiler-enforced layer separation, a rich domain model, and clear ubiquitous language.

---

## Target Solution Structure

```
SolaxHub.sln
├── src/
│   ├── SolaxHub.Domain/          # Pure domain — zero external dependencies
│   ├── SolaxHub.Application/     # Use cases — depends only on Domain
│   ├── SolaxHub.Infrastructure/  # Protocol adapters — implements domain interfaces
│   └── SolaxHub/                 # Host/composition root — wires everything
└── tests/
    └── SolaxHub.Integration.Tests/
```

---

## Phase 1 — SolaxHub.Domain (new project)

**`SolaxHub.Domain.csproj`** — no NuGet references except possibly `Microsoft.Extensions.Primitives` if needed. Zero framework dependencies.

### Common/
- **`AggregateRoot.cs`** — base class holding `IReadOnlyList<IDomainEvent> DomainEvents`, `AddDomainEvent()`, `ClearDomainEvents()`
- **`IDomainEvent.cs`** — empty marker interface

### Inverter/
#### Aggregate Root
**`Inverter.cs`** — the single Aggregate Root
```csharp
public sealed class Inverter : AggregateRoot
{
    public string SerialNumber { get; private set; }
    public InverterType Type { get; private set; }
    public InverterStatus Status { get; private set; }
    public InverterUseMode UseMode { get; private set; }
    public LockState LockState { get; private set; }
    public PowerControlMode PowerControlMode { get; private set; }
    public BatteryState Battery { get; private set; }
    public SolarState Solar { get; private set; }
    public GridState Grid { get; private set; }
    public int InverterPower { get; private set; }  // Watts (DC)
    public int HouseLoad => InverterPower - Grid.FeedInPower;
    public string RegistrationCode { get; private set; }

    // Only way to create/update — raises domain event
    public void Refresh(InverterSnapshot snapshot) { ... AddDomainEvent(new InverterDataRefreshed(this)); }
}
```

#### Value Objects (records)
**`BatteryState.cs`**
```csharp
public record BatteryState(
    int Power,           // Watts (negative=charge, positive=discharge)
    byte Capacity,       // 0-100%
    double OutputToday,  // kWh
    double InputToday,   // kWh
    double OutputTotal,  // kWh
    double InputTotal    // kWh
);
```

**`SolarState.cs`**
```csharp
public record SolarState(
    ushort Voltage1,   // 0.1V units
    ushort Current1,   // 0.1A units
    ushort Power1,     // Watts
    double EnergyToday, // kWh
    double EnergyTotal  // kWh
);
```

**`GridState.cs`**
```csharp
public record GridState(
    int FeedInPower,     // Watts (positive=export, negative=import)
    double FeedInEnergy, // kWh total exported
    double ConsumeEnergy // kWh total imported
);
```

**`InverterSnapshot.cs`** — input DTO to `Inverter.Refresh()`, carries raw-but-decoded values from the repository read:
```csharp
public record InverterSnapshot(
    string SerialNumber,
    InverterStatus Status,
    InverterUseMode UseMode,
    LockState LockState,
    PowerControlMode PowerControlMode,
    BatteryState Battery,
    SolarState Solar,
    GridState Grid,
    int InverterPower,
    ushort InverterVoltage,
    string RegistrationCode
);
```

#### Enums (renamed, no "Solax" prefix)
- **`InverterStatus.cs`** (14 values: WaitMode, NormalMode, etc.)
- **`InverterType.cs`** (24 values)
- **`InverterUseMode.cs`** (SelfUse, ForceTimeUse, BackUp, FeedInPriority, Unknown)
- **`LockState.cs`** (Locked=0, Unlocked=2014, UnlockedAdvanced=6868)
- **`PowerControlMode.cs`** (8 values)

#### Domain Events
**`Events/InverterDataRefreshed.cs`**
```csharp
public record InverterDataRefreshed(Inverter Inverter) : IDomainEvent;
```
**`Events/InverterUseModeChanged.cs`**
```csharp
public record InverterUseModeChanged(string SerialNumber, InverterUseMode NewMode) : IDomainEvent;
```

#### Repository Interface
**`IInverterRepository.cs`**
```csharp
public interface IInverterRepository
{
    Task<InverterSnapshot> ReadSnapshotAsync(CancellationToken ct);
    Task SetLockStateAsync(LockState state, CancellationToken ct);
    Task SetUseModeAsync(InverterUseMode mode, CancellationToken ct);
    Task SetPowerControlAsync(PowerControlMode mode, byte[] data, CancellationToken ct);
    Task SetBatteryMaxDischargeCurrentAsync(double amps, CancellationToken ct);
}
```

---

## Phase 2 — SolaxHub.Application (new project)

**`SolaxHub.Application.csproj`** — references Domain, adds MediatR.

**`DependencyInjection.cs`** — `AddApplication()` extension.

### Inverter/Commands/
Each command follows: command record + handler class using `IInverterRepository`.

**`RefreshInverterData/`**
- `RefreshInverterDataCommand.cs` — `IRequest` (no return)
- `RefreshInverterDataCommandHandler.cs`:
  1. `repository.ReadSnapshotAsync()`
  2. `inverter.Refresh(snapshot)` (aggregate updates state and raises `InverterDataRefreshed`)
  3. Dispatch domain events via `IPublisher`
  - The `Inverter` aggregate is held as a singleton service (`IInverterService`) so state persists between polls

**`SetInverterUseMode/`**
- `SetInverterUseModeCommand.cs` — `record(InverterUseMode Mode) : IRequest`
- Handler: calls `repository.SetUseModeAsync()`, raises `InverterUseModeChanged`

**`SetInverterLockState/`**
- `SetInverterLockStateCommand.cs`
- Handler: calls `repository.SetLockStateAsync()`

**`SetPowerControl/`**
- `SetPowerControlCommand.cs` — `record(PowerControlMode Mode, byte[] Data) : IRequest`
- Handler: calls `repository.SetPowerControlAsync()`

**`SetBatteryMaxDischargeCurrent/`**
- `SetBatteryMaxDischargeCurrentCommand.cs` — `record(double Amps) : IRequest`
- Handler: calls `repository.SetBatteryMaxDischargeCurrentAsync()`

### Domain Event Dispatch
`RefreshInverterDataCommandHandler` collects events from the aggregate and publishes them:
```csharp
foreach (var domainEvent in inverter.ClearDomainEvents())
    await publisher.Publish(domainEvent, ct);
```
MediatR notification handlers in Infrastructure subscribe to `InverterDataRefreshed` (replaces current `SolaxDataArrivedNotification`).

### Inverter State Service
**`IInverterStateService.cs`** / **`InverterStateService.cs`** — singleton holding the `Inverter` aggregate instance, so state persists across polls:
```csharp
public interface IInverterStateService
{
    Inverter Inverter { get; }
}
```

---

## Phase 3 — SolaxHub.Infrastructure (new project)

**`SolaxHub.Infrastructure.csproj`** — references Domain + Application, adds FluentModbus, Knx.Falcon.Sdk, etc.

### Modbus/
**`Client/ISolaxModbusClient.cs`** — kept (low-level Modbus interface)
**`Client/SolaxModbusClient.cs`** — kept (unchanged, internal)
**`Registers/InputRegisters.cs`** — all register addresses (moved here from domain)
**`Registers/HoldingRegisters.cs`** — holding register addresses
**`InverterRepository.cs`** — implements `IInverterRepository`:
- `ReadSnapshotAsync()` — makes all register reads (can be individual calls or batched), maps to `InverterSnapshot`
- All byte-order and scaling logic lives exclusively here
- All 22 read operations consolidated into one method
- Write methods: thin wrappers calling `ISolaxModbusClient` write methods

**`ModbusOptions.cs`** — renamed from `SolaxModbusOptions` (removes "Solax" prefix)

### Knx/
Files largely unchanged, just moved here:
- `Client/IKnxClient.cs`, `KnxClient.cs`
- `Services/IKnxValueBufferService.cs`, `KnxValueBufferService.cs`
- `Workers/KnxConnectionWorker.cs`, `KnxReceiverWorker.cs`
- `Options/KnxOptions.cs`
- **`Notifications/InverterDataRefreshedKnxHandler.cs`** — renames from `KnxSolaxDataNotificationHandler`, subscribes to `InverterDataRefreshed` domain event
- **`Requests/`** — KnxReadValueRequest/Handler, KnxWriteValueRequest/Handler (unchanged)

### Udp/
- **`Notifications/InverterDataRefreshedUdpHandler.cs`** — renames from `UdpSolaxDataNotificationHandler`
- `Options/UdpOptions.cs`

---

## Phase 4 — SolaxHub (host, refactored)

### Workers/
**`InverterPollingWorker.cs`** — replaces `SolaxModbusWorker`:
1. Ensures Modbus connection is alive
2. Sends `RefreshInverterDataCommand` via `ISender`
3. Checks lock state: if `Locked`, sends `SetInverterLockStateCommand(UnlockedAdvanced)` first
4. Delays by configured interval

### Extensions/
**`ServiceCollectionExtensions.cs`** — root `AddSolaxHub()`:
```csharp
services
    .AddDomain()          // registers InverterStateService singleton
    .AddApplication()     // registers MediatR + command handlers
    .AddInfrastructure()  // registers repository, clients, KNX, UDP
    .AddWorkers();        // registers InverterPollingWorker, KnxConnectionWorker
```

---

## Critical File Paths (Current → New)

| Current | New |
|---|---|
| `SolaxHub/Solax/Models/SolaxData.cs` | `SolaxHub.Domain/Inverter/Inverter.cs` (Aggregate) |
| `SolaxHub/Solax/Models/SolaxInverterStatus.cs` | `SolaxHub.Domain/Inverter/InverterStatus.cs` |
| `SolaxHub/Solax/Models/SolaxInverterUseMode.cs` | `SolaxHub.Domain/Inverter/InverterUseMode.cs` |
| `SolaxHub/Solax/Models/SolaxLockState.cs` | `SolaxHub.Domain/Inverter/LockState.cs` |
| `SolaxHub/Solax/Models/SolaxPowerControlMode.cs` | `SolaxHub.Domain/Inverter/PowerControlMode.cs` |
| `SolaxHub/Solax/Notifications/SolaxDataArrivedNotification.cs` | `SolaxHub.Domain/Inverter/Events/InverterDataRefreshed.cs` |
| `SolaxHub/Solax/Queries/Get*.cs` (22 files) | **Eliminated** — logic folded into `InverterRepository.ReadSnapshotAsync()` |
| `SolaxHub/Solax/Commands/Set*.cs` (4 files) | `SolaxHub.Application/Inverter/Commands/Set*/` |
| `SolaxHub/Solax/Services/SolaxPollingService.cs` | Logic split: loop→`InverterPollingWorker`, reads→`InverterRepository`, orchestration→`RefreshInverterDataCommandHandler` |
| `SolaxHub/Solax/Modbus/Client/ISolaxModbusClient.cs` | `SolaxHub.Infrastructure/Modbus/Client/ISolaxModbusClient.cs` |
| `SolaxHub/Knx/Notifications/Handlers/KnxSolaxDataNotificationHandler.cs` | `SolaxHub.Infrastructure/Knx/Notifications/InverterDataRefreshedKnxHandler.cs` |
| `SolaxHub/Udp/Notifications/Handlers/UdpSolaxDataNotificationHandler.cs` | `SolaxHub.Infrastructure/Udp/Notifications/InverterDataRefreshedUdpHandler.cs` |
| `SolaxHub/Solax/Workers/SolaxModbusWorker.cs` | `SolaxHub/Workers/InverterPollingWorker.cs` |

---

## Data Flow (After Refactoring)

```
InverterPollingWorker (Host)
  │  sends RefreshInverterDataCommand (Application)
  ▼
RefreshInverterDataCommandHandler (Application)
  │  calls IInverterRepository.ReadSnapshotAsync()
  ▼
InverterRepository (Infrastructure/Modbus)
  │  reads Modbus registers → InverterSnapshot
  ▼
RefreshInverterDataCommandHandler
  │  calls Inverter.Refresh(snapshot) → raises InverterDataRefreshed
  │  dispatches domain events via IPublisher
  ▼
InverterDataRefreshedKnxHandler (Infrastructure/Knx)
InverterDataRefreshedUdpHandler (Infrastructure/Udp)
```

---

## Key DDD Patterns Demonstrated

1. **Aggregate Root** (`Inverter`) — single entry point for state changes, raises domain events
2. **Value Objects** (`BatteryState`, `SolarState`, `GridState`) — immutable, equality by value
3. **Domain Events** (`InverterDataRefreshed`) — raised by aggregate, not infrastructure
4. **Repository Pattern** (`IInverterRepository`) — domain never knows about Modbus registers
5. **Ubiquitous Language** — "Solax" prefix removed from domain types; names match domain concepts
6. **Layer Isolation** — enforced by project references (Domain has zero deps; Infrastructure cannot leak into Domain)
7. **Application Services** — command handlers orchestrate domain objects without containing business logic

---

## Integration Test Updates

- `SolaxHubFixture` wire `IInverterRepository` mock instead of `ISolaxModbusClient` in unit tests
- `InverterRepository` integration tests test the mapping logic in isolation
- Existing query tests `SolaxQueryTests` are replaced with `InverterRepositoryTests` that verify `ReadSnapshotAsync()` returns correct `InverterSnapshot`
- Polling service tests become `RefreshInverterDataCommandHandlerTests`

---

## Implementation Order

1. Create `SolaxHub.Domain` project — domain types, no dependencies
2. Create `SolaxHub.Application` project — commands + handlers
3. Create `SolaxHub.Infrastructure` project — Modbus repo + KNX/UDP handlers
4. Update `SolaxHub` host — workers, composition root
5. Update `SolaxHub.Integration.Tests` — fixture, test renames
6. Delete old `SolaxHub/Solax/` folder contents
7. Update `SolaxHub.sln` — add new projects, src/tests layout

---

## Verification

```bash
# Build all projects
dotnet build SolaxHub.sln

# Run tests
dotnet test SolaxHub.sln

# Verify Domain project has zero framework dependencies
dotnet list SolaxHub.Domain/SolaxHub.Domain.csproj package

# Run the application
dotnet run --project src/SolaxHub/SolaxHub.csproj
```

Confirm:
- `SolaxHub.Domain` has no NuGet packages
- `SolaxHub.Application` only references Domain + MediatR
- `SolaxHub.Domain` cannot reference Infrastructure (compiler-enforced)
- Integration tests pass with mocked `IInverterRepository`

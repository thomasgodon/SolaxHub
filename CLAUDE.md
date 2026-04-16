# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build SolaxHub.sln

# Run all tests
dotnet test SolaxHub.sln

# Run a single test
dotnet test SolaxHub.Integration.Tests/SolaxHub.Integration.Tests.csproj --filter "FullyQualifiedName~SolaxQueryTests"

# Run the application
dotnet run --project SolaxHub/SolaxHub.csproj

# Publish (linux-arm64)
dotnet publish SolaxHub/SolaxHub.csproj -r linux-arm64 -c Release
```

## Architecture Overview

SolaxHub is a .NET 9 Worker Service that bridges a **Solax solar inverter** (via Modbus TCP) to multiple output integrations: **KNX** home automation, **Azure IoT Hub**, and **UDP**. The data flow is:

```
Solax Inverter (Modbus TCP)
        ↓
  SolaxModbusWorker  (polls on configurable interval)
        ↓
  SolaxPollingService  (sends Queries → builds SolaxData → publishes notification)
        ↓
  SolaxDataArrivedNotification  (MediatR INotification)
        ↓
  ┌─────────────────────────────────────┐
  │ KnxSolaxDataNotificationHandler     │  → writes KNX group addresses via KnxClient
  │ IotHubSolaxDataNotificationHandler  │  → sends telemetry to Azure IoT Hub
  │ UdpSolaxDataNotificationHandler     │  → broadcasts UDP packets
  └─────────────────────────────────────┘
```

**KNX integration** also has two background workers: `KnxConnectionWorker` (maintains connection) and `KnxReceiverWorker` (handles incoming KNX read requests). KNX group addresses are configured as `ReadGroupAddresses` (inverter data → KNX bus) and `WriteGroupAddresses` (KNX bus → inverter commands). Incoming KNX read requests are served from `KnxValueBufferService` (in-memory cache of latest values).

**Projects:**
- `SolaxHub/` — main Worker Service (entry point: `Program.cs`)
- `SolaxHub.Integration.Tests/` — integration tests using `SolaxHubFixture` which wires real DI with mocked external clients (`ISolaxModbusClient`, `IKnxClient`, `IIotHubDevicesService`)

**Domain folders** under `SolaxHub/`:
- `Solax/` — Modbus client, queries/commands for all inverter registers, polling service, data models
- `Knx/` — Falcon SDK client, value buffer service, notification/request handlers, workers
- `IotHub/` — Azure DPS provisioning + device client, notification handler
- `Udp/` — UDP sender, notification handler
- `Extensions/` — root `AddSolaxHub()` composition root

**Configuration** (`appsettings.json`): `SolaxModbusOptions` (host, port, poll interval), `KnxOptions` (enabled, host, port, group address mappings), `IotHubOptions` (array of IoT devices with DPS credentials), `UdpOptions` (enabled, host, port mapping). Features are gated by `Enabled` boolean flags — handlers return early when disabled.

## General
- Target .NET 10.0 for all projects.
- Enable nullable reference types and implicit usings.
- Use dependency injection for all services and clients.
- Use MediatR for request/response and command/query patterns.
- Follow CQRS (Command Query Responsibility Segregation) principles.
- Implement event-driven architecture using MediatR notifications.

## Architecture Patterns

### CQRS Pattern
- Separate Commands (write operations) from Queries (read operations)
- Commands implement `IRequest` (void return)
- Queries implement `IRequest<TResult>` (typed return)
- All handlers reside in dedicated `Handlers` folders

### MediatR Usage
- Commands: `IRequestHandler<TCommand>`
- Queries: `IRequestHandler<TQuery, TResult>`
- Notifications: `INotificationHandler<TNotification>`
- Inject `ISender` for requests and `IPublisher` for notifications

### Event-Driven Architecture
- Use `INotification` for domain events
- Support multiple notification handlers for the same event
- Implement pub-sub pattern for cross-cutting concerns

## Naming Conventions

### General
- Use `PascalCase` for class, interface, and method names.
- Prefix interfaces with `I` (e.g., `ISolaxPollingService`).
- Use `camelCase` for local variables and parameters.
- Suffix test classes with `Tests`.

### Handlers
- Command handlers: `{CommandName}CommandHandler`
- Query handlers: `{QueryName}QueryHandler`
- Notification handlers: `{Domain}{NotificationName}NotificationHandler`

### Requests/Responses
- Commands: `{Action}{Entity}Command` (e.g., `SetBatteryDischargeMaxCurrentCommand`)
- Queries: `Get{Property}Query` (e.g., `GetBatteryCapacityQuery`)
- Notifications: `{Entity}DataArrivedNotification`

### Services & Clients
- Service interfaces: `I{Domain}{Purpose}Service`
- Client interfaces: `I{Domain}Client`
- Extension methods: `{Domain}Extensions`

### Configuration
- Options classes: `{Domain}Options`
- Configuration sections match class names exactly

## Project Structure

### Organization
- Place background/worker services in a `Workers` folder and inherit from `BackgroundService`
- Place integration tests in the `SolaxHub.Integration.Tests` project
- Use `Fixtures` for test setup and dependency configuration
- Group related functionality in domain folders (e.g., `Solax`, `Knx`, `IotHub`)

### Folder Structure
```
Domain/
├── Commands/
│   └── Handlers/
├── Queries/
│   └── Handlers/
├── Notifications/
│   └── Handlers/
├── Models/
├── Services/
├── Extensions/
└── Workers/
```

## Data Models

### Record Types
- Use `record` types for immutable data models
- Use `required` properties for mandatory fields
- Use `init` accessors for initialization-only properties
- Implement computed properties for derived values

### Enums
- Use descriptive enum names with explicit values
- Create extension methods for enum conversions
- Use pattern matching for enum-to-value conversions

### Value Objects
- Create strongly-typed wrappers for domain values
- Ensure immutability by design
- Provide conversion methods between related types

## Service Layer

### Client Abstraction
- Use interface-based client abstractions
- Implement async operations with `CancellationToken`
- Manage connection state properly
- Follow resource disposal patterns

### Service Composition
- Orchestrate services via dependency injection
- Handle cross-cutting concerns via notification handlers
- Ensure thread-safe operations for stateful services

### Buffer/Cache Patterns
- Use thread-safe operations with `Lock`
- Implement value comparison to prevent unnecessary updates
- Use dictionary-based caching strategies

## Configuration

### Options Pattern
- Use strongly-typed configuration with `IOptions<T>`
- Mark required properties with `required` keyword
- Provide default values for optional settings
- Name configuration sections to match option class names

### Service Registration
- Use modular registration via extension methods
- Create domain-specific `ServiceCollectionExtensions`
- Manage service lifetimes consistently (Singleton for stateful services)
- Bind configuration in extension methods

### Feature Flags
- Use boolean `Enabled` properties in options
- Implement early exit patterns when features disabled
- Apply conditional service execution

## Testing

### Test Framework
- Use xUnit for unit and integration tests
- Use Moq for mocking dependencies
- Use FluentAssertions for assertions
- Follow AAA pattern (Arrange, Act, Assert)

### Test Organization
- Create base test classes for shared functionality
- Use fixture pattern for test setup and teardown
- Organize tests by domain

### Naming & Structure
- Name test methods with `Given_When_Then` or similar descriptive patterns
- Use `[Fact]` for single-case tests and `[Theory]` with `[InlineData]` for parameterized tests
- Verify all mocks in assertions using `VerifyAll()`

### Mock Strategy
- Mock all external dependencies
- Use service replacement via `ReplaceWithMock<T>()` extension
- Use singleton mock lifetime for stateful scenarios
- Provide explicit test data for boundary conditions

## Async & Await
- Use async/await for all I/O-bound operations
- Suffix async methods with `Async`
- Pass `CancellationToken` to all async methods
- Handle cancellation gracefully in background services

## Error Handling & Logging

### Exception Handling
- Use try-catch blocks with specific exception logging
- Provide contextual error messages with structured logging
- Implement graceful degradation (return null/empty instead of throwing)
- Never swallow exceptions without logging

### Connection Resilience
- Check connection state before operations
- Implement retry logic in background workers
- Provide DNS resolution fallback patterns

### Validation
- Use `Math.Clamp()` for numeric bounds validation
- Implement null checks with early returns
- Use type validation with pattern matching

### Logging Patterns
- Use structured logging with `ILogger<T>`
- Apply appropriate log levels (Trace, Debug, Information, Warning, Error)
- Use named parameters in log messages
- Integrate with OpenTelemetry for observability

### Activity Tracing
- Use static `ActivitySource` fields in classes
- Follow consistent naming: `ActivitySource ActivitySource = new(nameof(ClassName))`
- Use `using` statements for activity scope management

## Background Services

### Worker Pattern
- Inherit from `BackgroundService`
- Implement consistent polling loops with configurable intervals
- Handle graceful cancellation token processing
- Use `Stopwatch` for timing operations

### Resource Management
- Properly dispose of connections and resources
- Implement connection state checking
- Handle reconnection scenarios gracefully

## Integration Patterns

### Protocol Abstraction
- Create domain-specific clients (Modbus, KNX, IoT Hub)
- Use unified data models across protocols
- Implement extension methods for data transformation

### Message Transformation
- Provide type-safe conversions between domains
- Normalize enums for different systems
- Handle byte-level data manipulation patterns

## Documentation & Comments
- Use XML documentation for public APIs
- Add comments for complex logic or non-obvious code
- Document configuration options and their purposes

## Observability

### OpenTelemetry Integration
- Configure Azure Monitor integration for traces, metrics, and logs
- Set up resource configuration with service naming
- Use structured logging with OpenTelemetry

### Activity Instrumentation
- Implement consistent activity naming and scoping
- Track performance in critical paths
- Enable distributed tracing capabilities

## Miscellaneous
- Use `required` properties for record types where appropriate
- Use `internal` for types not intended for public use
- Use `readonly` for fields that are not reassigned
- Store configuration in `appsettings.json` and use `IOptions<T>` for access
- Implement feature flags using boolean `Enabled` properties
- Use consistent lifetime management for dependency injection

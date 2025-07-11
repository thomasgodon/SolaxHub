# Coding & Project Rules

## General
- Target .NET 9.0 for all projects.
- Enable nullable reference types and implicit usings.
- Use dependency injection for all services and clients.
- Use MediatR for request/response and command/query patterns.

## Naming Conventions
- Use `PascalCase` for class, interface, and method names.
- Prefix interfaces with `I` (e.g., `ISolaxPollingService`).
- Use `camelCase` for local variables and parameters.
- Suffix test classes with `Tests`.

## Project Structure
- Place background/worker services in a `Workers` folder and inherit from `BackgroundService`.
- Place integration tests in the `SolaxHub.Integration.Tests` project.
- Use `Fixtures` for test setup and dependency configuration.

## Testing
- Use xUnit for unit and integration tests.
- Use Moq for mocking dependencies.
- Use FluentAssertions for assertions.
- Name test methods with `Given_When_Then` or similar descriptive patterns.
- Use `[Fact]` for single-case tests and `[Theory]` with `[InlineData]` for parameterized tests.
- Verify all mocks in assertions.

## Async & Await
- Use async/await for all I/O-bound operations.
- Suffix async methods with `Async`.
- Pass `CancellationToken` to all async methods.

## Exception Handling & Logging
- Use structured logging with `ILogger<T>`.
- Log exceptions with context and use log levels appropriately.
- Avoid swallowing exceptions; handle or log them.

## Documentation & Comments
- Use XML documentation for public APIs.
- Add comments for complex logic or non-obvious code.

## Patterns & Practices
- Use records for immutable data models.
- Use dependency injection for all services.
- Prefer constructor injection.
- Use MediatR for decoupling business logic from controllers/services.

## Configuration
- Store configuration in `appsettings.json` and use `IOptions<T>` for access.

## Miscellaneous
- Use `required` properties for record types where appropriate.
- Use `internal` for types not intended for public use.
- Use `readonly` for fields that are not reassigned.

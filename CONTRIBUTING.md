# Contributing to TermBlade

Bug fixes and feature suggestions are always welcome. For bug fixes, open a PR for review.
Feature suggestions are subject to discussion via issues.

## Prerequisites

- [.NET 10](https://dotnet.microsoft.com/download/dotnet/10.0) or later

## Build

```bash
dotnet restore
dotnet build
```

## Test

```bash
dotnet test
```

## Run Samples

```bash
dotnet run --project samples/TermBlade.Samples -- layout
dotnet run --project samples/TermBlade.Samples -- editor
```

## Project Structure

| Path | Purpose |
|---|---|
| `src/TermBlade.Core/` | Core library — all public API |
| `tests/TermBlade.Tests/` | xUnit tests — cover every public API |
| `samples/TermBlade.Samples/` | Console app samples demonstrating features |

## Code Style

- Follow standard C# conventions (PascalCase for types/members, camelCase for locals/fields)
- Use `readonly struct` for value types where appropriate
- Use `IDisposable` for types that own resources
- XML doc comments (`/// <summary>`) for public APIs where the intent is non-obvious
- No JSDoc-style block comments

## Code of Conduct

- Treat everyone with respect and empathy.
- Be kind, constructive, and assume good intent.
- Critique code, not people.
- Follow project guidelines and maintainers' decisions.

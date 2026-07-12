# Contributing to TermBlade

Bug fixes and feature suggestions are always welcome. For bug fixes, open a PR for review. Feature suggestions are subject to discussion via issues. Keep pull requests focused so terminal regressions remain easy to review.

## Prerequisites

- [.NET 10](https://dotnet.microsoft.com/download/dotnet/10.0) or later

## Build

```bash
dotnet restore TermBlade.slnx
dotnet build TermBlade.slnx --no-restore
dotnet format TermBlade.slnx --verify-no-changes --verbosity minimal
```

## Test

```bash
dotnet test TermBlade.slnx --no-restore
```

The suite includes rendering, input, buffer, Razor hosting, and documentation-inventory tests. A public behavior change must include a focused xUnit test. A change to the interactive docs also requires `npm ci && npm run build` from `docs/TermBlade.Docs.Wasm`.

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
| `docs/` | Static and interactive documentation |

## Code Style

- Follow standard C# conventions (PascalCase for types/members, camelCase for locals/fields)
- Use `readonly struct` for value types where appropriate
- Use `IDisposable` for types that own resources
- XML doc comments (`/// <summary>`) for public APIs where the intent is non-obvious
- No JSDoc-style block comments
- Avoid per-cell allocations in renderer hot paths; see [`docs/performance.md`](docs/performance.md)
- Keep architecture and lifecycle changes reflected in [`docs/architecture.md`](docs/architecture.md)

## Pull requests

Describe the user-visible behavior, the tests run, and any terminal/platform limitation. For rendering changes, explain the ANSI diff behavior and mention whether a sample was run. Do not include generated `bin/`, `obj/`, or npm dependency output.

## Code of Conduct

- Treat everyone with respect and empathy.
- Be kind, constructive, and assume good intent.
- Critique code, not people.
- Follow project guidelines and maintainers' decisions.

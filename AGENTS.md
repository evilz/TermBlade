# TermBlade - AI Agent Instructions

Welcome to the TermBlade repository. This document provides essential context, architectural principles, and coding standards to guide AI agents working on this project.

## Project Overview

TermBlade is a terminal UI core library for modern .NET. It provides a cell-based rendering engine with ANSI escape sequence support, styled text, a rope-backed edit buffer with full undo/redo, a plugin architecture, and sample console applications.

## Solution Layout

The repository is structured as follows:

- `src/TermBlade.Core/`: Core library containing all public APIs (e.g., Ansi, Buffer, Text, Rendering).
- `src/TermBlade.Razor/`: Razor host + component wrappers for renderables.
- `tests/TermBlade.Tests/`: xUnit tests that must cover every public API.
- `samples/TermBlade.Samples/`: Console application samples demonstrating features.
- `samples/TermBlade.Razor.Samples/`: Razor-hosted sample console applications.

## Agent Guidelines & Workflow

When tasked with implementing features, fixing bugs, or refactoring:

1. **Understand the Goal**: Review the relevant issues, files, and project layout. Ensure your changes align with the core rendering engine or text buffer architecture.
2. **Build and Test First**: Always verify that the project builds and existing tests pass before making changes. Run the tests from the repository root using:
   - `dotnet restore`
   - `dotnet build`
   - `dotnet test`
3. **Run Samples**: To manually verify visual or behavioral changes, run the sample apps.
   - Example: `dotnet run --project samples/TermBlade.Samples -- layout`
4. **Test-Driven Changes**: Write or update xUnit tests in `tests/TermBlade.Tests/` for any new functionality or bug fix. Test coverage is critical.

## Code Style & Conventions

- **Language**: Modern C# (targeting .NET 9 / 10).
- **Naming Conventions**: 
  - `PascalCase` for classes, structs, records, methods, and properties.
  - `camelCase` for local variables and method parameters.
  - `_camelCase` for private fields (standard C# conventions).
- **Value Types**: Use `readonly struct` for value types where appropriate to ensure immutability and performance.
- **Resource Management**: Implement `IDisposable` (and use `using` statements/declarations) for types that own unmanaged resources or event subscriptions.
- **Documentation**: Provide XML doc comments (`/// <summary>`) for public APIs where the intent is non-obvious. Do not use JSDoc-style block comments.
- **File Structure**: Keep one primary type per file. Ensure namespaces map cleanly to the folder structure.

## Key Architectural Concepts

- **Rgba / ColorIntent**: Used for packed color representations (RGB, ANSI-256 indexed, or terminal-default).
- **CellBuffer**: A 2D terminal cell grid handling text, borders, blitting, and alpha blending.
- **TextBuffer / EditBuffer**: Text content management. `EditBuffer` uses a rope-backed data structure supporting cursors, insertions, deletions, and undo/redo operations.
- **Renderer**: Diff-based ANSI terminal renderer with alternate-screen support. Optimize for minimal terminal updates.
- **SyntaxStyle**: Named style definitions with priority-aware merging.

## Working with AI

When interacting with a user:
- Be concise and provide code changes that strictly adhere to the project's design paradigms.
- Preserve existing comments and docstrings unless they are directly rendered obsolete by your code changes.
- Prioritize performance when modifying the renderer or cell buffers, as efficiency is crucial for a TUI library.

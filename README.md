# TermBlade

<div align="center">
    <img src="/assets/TermBlade.png" alt="TermBlade" height="120" />
    <br/><br/>
    <em>Razor-sharp Terminal UIs</em>
    <br/><br/>
    <a href="https://www.nuget.org/packages/TermBlade.Core"><img alt="NuGet" src="https://img.shields.io/nuget/v/TermBlade.Core?style=flat-square" /></a>
    <a href="https://github.com/evilz/TermBlade/actions/workflows/build-dotnet.yml"><img alt="Build" src="https://img.shields.io/github/actions/workflow/status/evilz/TermBlade/build-dotnet.yml?style=flat-square&branch=main" /></a>
</div>

TermBlade is a terminal UI core library for **.NET 10**. It provides a cell-based rendering engine with ANSI escape sequence support, styled text, a rope-backed edit buffer with full undo/redo, a plugin architecture, and sample console applications.

## Requirements

- [.NET 10](https://dotnet.microsoft.com/download/dotnet/10.0) or later

## Quick Start

```bash
dotnet add package TermBlade.Core
```

```csharp
using TermBlade.Core.Ansi;
using TermBlade.Core.Buffer;

using var buf = CellBuffer.Create(60, 20);
var bg     = Rgba.FromInts(0,   0,   0);
var fg     = Rgba.FromInts(255, 255, 255);
var accent = Rgba.FromInts(0,   200, 255);

buf.Clear(bg);
buf.DrawBox(0, 0, 60, 20, accent, bg, BorderStyle.Rounded, BorderSides.All,
            fill: true, title: " My App ");
buf.DrawText("Hello from TermBlade!", 4, 3, fg, bg);
```

## Solution Layout

```
src/TermBlade.Core/            # Core library (NuGet package)
  Ansi/                        # Rgba, AnsiCodes, TextAttributes, ColorIntent
  Buffer/                      # CellBuffer, Cell, BorderStyle
  Text/                        # TextBuffer, EditBuffer, Rope, StyledText
  Rendering/                   # Renderer (diff-based, alternate screen)
  Syntax/                      # SyntaxStyle, StyleDefinition
  Events/                      # EventEmitter
  Plugins/                     # IPlugin, PluginRegistry
src/TermBlade.Razor/           # Razor host + component wrappers for renderables
tests/TermBlade.Tests/         # xUnit test project (126 tests)
samples/TermBlade.Samples/    # Sample console applications
samples/TermBlade.Razor.Samples/ # Razor-hosted sample console applications
docs/TermBlade.Docs.Wasm/     # Interactive docs (Blazor WASM + xterm.js)
```

## Build & Test

```bash
dotnet restore
dotnet build
dotnet test
```

## Run Samples

```bash
dotnet run --project samples/TermBlade.Samples -- layout   # simple box layout
dotnet run --project samples/TermBlade.Samples -- styled   # text attributes
dotnet run --project samples/TermBlade.Samples -- editor   # edit buffer demo
dotnet run --project samples/TermBlade.Samples -- scroll   # scrolling content
dotnet run --project samples/TermBlade.Samples -- input    # keyboard input
dotnet run --project samples/TermBlade.Razor.Samples -- layout   # Razor-hosted layout demo
dotnet run --project samples/TermBlade.Razor.Samples -- editor   # Razor-hosted editor demo
dotnet run --project samples/TermBlade.Razor.Samples -- console  # Razor-hosted overlay demo
dotnet run --project samples/TermBlade.Razor.Samples -- calendar # Razor-hosted calendar demo
dotnet run --project samples/TermBlade.Gallery                 # Interactive component gallery (includes Calendar demo)
```

## Documentation

Visit the [TermBlade documentation site](https://evilz.github.io/TermBlade/) for detailed guides and API reference.

### Interactive Documentation (xterm.js + Blazor WASM)

An interactive terminal-based documentation app is available in `docs/TermBlade.Docs.Wasm/`.
It uses **xterm.js** as a rendering surface and **Blazor WebAssembly** for a fake shell
that demonstrates TermBlade components with ANSI output — no backend, no OS access.

```bash
# Run locally
cd docs/TermBlade.Docs.Wasm
npm install && npm run build
dotnet run
```

The interactive docs are automatically deployed to GitHub Pages. To enable deployment:

1. Go to **Settings → Pages** in your repository.
2. Under **Source**, select **GitHub Actions**.

See [`docs/TermBlade.Docs.Wasm/README.md`](docs/TermBlade.Docs.Wasm/README.md) for details.

## Key Types

| Type | Description |
|---|---|
| `Rgba` | Packed color: RGB, ANSI-256 indexed, or terminal-default intent |
| `CellBuffer` | 2D terminal cell grid with text, borders, blitting, alpha blend |
| `TextBuffer` | Styled text storage (read-only display) |
| `EditBuffer` | Rope-backed editor: cursor, insert/delete, undo/redo |
| `Renderer` | Diff-based ANSI terminal renderer with alternate-screen support |
| `SyntaxStyle` | Named style definitions with priority-aware merging |
| `EventEmitter` | Simple on/off/once/emit event bus |
| `PluginRegistry` | `IPlugin` registration and lifecycle |

# TermBlade Interactive Documentation

This folder contains the **Blazor WebAssembly interactive documentation** for TermBlade.

## Architecture

```
TermBlade.Docs.Wasm/
├── Terminal/                    # Core fake terminal infrastructure
│   ├── ITerminalOutput.cs       # Output abstraction (testable)
│   ├── ITerminalCommand.cs      # Command interface
│   ├── FakeShell.cs             # Command processor (no OS access)
│   ├── Commands/                # Command implementations
│   │   ├── HelpCommand.cs
│   │   ├── ClearCommand.cs
│   │   ├── ComponentsCommand.cs
│   │   ├── DemoCommand.cs
│   │   └── AboutCommand.cs
│   └── Interop/
│       └── XtermInterop.cs      # Blazor ↔ xterm.js bridge
├── Pages/
│   ├── Home.razor               # Redirects to /terminal
│   ├── TerminalPage.razor       # Main terminal page
│   └── NotFound.razor
├── wwwroot/
│   ├── terminal/
│   │   └── terminal.ts          # xterm.js TypeScript module (source)
│   └── dist/
│       ├── terminal.js          # Bundled JS (built by Vite)
│       └── xterm.css            # xterm.js styles
├── vite.config.ts               # Vite build configuration
├── tsconfig.json                # TypeScript configuration
└── package.json                 # npm dependencies
```

## Why xterm.js + Blazor WebAssembly?

| Concern | Solution |
|---------|----------|
| Terminal rendering | **xterm.js** — battle-tested terminal emulator with full ANSI support |
| Keyboard input | xterm.js `onData` → forwarded to .NET via JS interop |
| Command processing | **C# FakeShell** — runs entirely in the browser via WebAssembly |
| Component demos | ANSI escape sequences generated in C# → rendered by xterm.js |
| No backend needed | Blazor WASM is a standalone static app |
| GitHub Pages compatible | Published as static files with `.nojekyll` |

### Why not a real terminal?

TermBlade is a TUI library — but the documentation doesn't need a real terminal.
Instead, xterm.js provides:
- A pixel-perfect terminal rendering surface
- Full ANSI escape sequence support (colors, styles, cursor control)
- Keyboard input handling
- Scrollback buffer
- Web links detection
- Auto-fit to container

The "shell" is entirely fake — written in C#, it processes commands and generates
ANSI output to demonstrate TermBlade's components visually.

## Local Development

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 20+](https://nodejs.org/)

### Build and Run

```bash
# 1. Install npm dependencies
cd docs/TermBlade.Docs.Wasm
npm install

# 2. Build the JS bundle
npm run build

# 3. Run the Blazor WASM app
dotnet run
```

The app will be available at `http://localhost:5000` (or the port shown in output).

### Rebuild JS after TypeScript changes

```bash
cd docs/TermBlade.Docs.Wasm
npm run build
```

## Available Commands

| Command | Description |
|---------|-------------|
| `help` | Show available commands |
| `clear` | Clear the terminal screen |
| `components` | List all TermBlade components |
| `demo <name>` | Show a visual demo (button, table, tree, box, chart, spinner, calendar, text) |
| `about` | Show information about TermBlade |

## Adding New Commands

1. Create a class implementing `ITerminalCommand` in `Terminal/Commands/`.
2. Register it in `TerminalPage.razor`'s `OnAfterRenderAsync`.

```csharp
public class MyCommand : ITerminalCommand
{
    public string Name => "mycommand";
    public string Description => "Does something cool";

    public async Task ExecuteAsync(string[] args, ITerminalOutput output)
    {
        await output.WriteLineAsync("\x1b[1;33mHello from my command!\x1b[0m");
    }
}
```

## GitHub Pages Deployment

The documentation is automatically deployed to GitHub Pages when changes are pushed
to the `main` branch (files in `docs/TermBlade.Docs.Wasm/` or `src/TermBlade.Core/`).

### Manual Setup

1. Go to **Settings → Pages** in your GitHub repository.
2. Under **Source**, select **GitHub Actions**.
3. The `deploy-docs-wasm.yml` workflow will handle the rest.

### Manual Deployment

You can also trigger a deployment manually:

1. Go to **Actions** → **Deploy Docs to GitHub Pages**.
2. Click **Run workflow**.

## Design Decisions

- **No `node-pty`**: The terminal is purely visual — no OS process spawning.
- **No WebSocket**: All communication is via Blazor JS interop (synchronous in-browser).
- **No backend**: The app is a standalone Blazor WASM app — pure static files.
- **xterm.js is isolated**: Only used in the docs layer, never in TermBlade.Core.
- **Commands are testable**: `ITerminalOutput` can be mocked for unit testing.
- **Vite bundling**: xterm.js and addons are bundled into a single ES module.

## Future Extensions

- [ ] Mouse event support via xterm.js mouse tracking
- [ ] Live component rendering (instantiate real TermBlade renderables → CellBuffer → ANSI)
- [ ] Search/filter in component gallery
- [ ] Multi-tab terminal support
- [ ] Command history (up/down arrow navigation)
- [ ] Tab completion

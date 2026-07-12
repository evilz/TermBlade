# TermBlade agent guide

## Purpose

TermBlade is a .NET 10 terminal UI library. The performance-sensitive path is:

`Renderable -> Layout -> RenderBuffer/CellBuffer -> ANSI diff -> terminal`.

The repository also contains a Razor host, sample applications, two command-line tools, an interactive documentation site, and xUnit tests.

## Repository map

- `src/TermBlade.Core`: public rendering, ANSI, buffer, text, layout, input, plugin, and renderable APIs.
- `src/TermBlade.Razor`: Razor component wrappers and terminal hosting integration.
- `src/TermBlade.FileManager`, `src/TermBlade.CsvViewer`, `src/TermBlade.Chess`: products/tools built on the library.
- `tests/TermBlade.Tests`: unit, rendering, integration, and documentation consistency tests.
- `samples`: runnable console and Razor examples.
- `docs`: static documentation and the Blazor WebAssembly interactive site.

## Required workflow

Inspect the relevant implementation, tests, project file, and public API documentation before changing code. Preserve unrelated user changes in a dirty worktree.

Run the checks from the repository root:

```powershell
dotnet restore TermBlade.slnx
dotnet build TermBlade.slnx --no-restore
dotnet test TermBlade.slnx --no-restore
dotnet format TermBlade.slnx --verify-no-changes --verbosity minimal
```

For the interactive site, also run `npm ci` and `npm run build` in `docs/TermBlade.Docs.Wasm`. Add or update xUnit tests for every behavior change. Run a sample when terminal layout, keyboard input, ANSI output, or Razor hosting changes.

## Design rules

- Target `net10.0`, nullable reference types, implicit usings, and the repository's two-space indentation.
- Keep one primary type per file and namespaces aligned with folders.
- Public APIs require useful XML documentation, including parameters, return values, exceptions, and lifecycle behavior where applicable.
- Prefer immutable `readonly struct` value types for small value objects and validate public arguments at the boundary.
- Use `IDisposable` for terminal modes, event subscriptions, buffers, and other owned resources. Disposal must be idempotent.
- Keep renderables deterministic: layout computes geometry, rendering writes cells, and input handlers update state/request a render.
- Do not block asynchronous work in Razor or WebAssembly with `.Result`, `.Wait()`, or `GetAwaiter().GetResult()`.
- Avoid string-based component resolution in publishable WASM code; use static type references or trimming annotations.
- Preserve cross-platform behavior for terminal input, dimensions, paths, and ANSI capabilities.

## Performance rules

Optimize measured hot paths, especially frame rendering and text editing. Avoid per-cell `TextWriter`, LINQ, regex, or intermediate string allocations. Prefer reusable `StringBuilder`, spans in synchronous code, and `Memory<T>` across asynchronous boundaries. Do not introduce `unsafe`, pooling, or `ValueTask` without evidence and a correctness test.

When changing rendering, test unchanged-frame diffing and full-frame output. ANSI sequence changes must retain exact ordering and reset semantics. Benchmark before claiming an improvement when a change is more than a local allocation reduction.

## Documentation rules

Documentation follows Diátaxis: tutorials teach a first successful application; how-to guides solve one concrete task; reference documents describe APIs and invariants; explanations describe architecture and trade-offs.

Keep README examples copy/pasteable and consistent with the current public API. Update `docs/architecture.md` or `docs/performance.md` when architecture or hot-path rules change. Do not claim support for a framework, command, or sample that has not been built or tested.

## Review checklist

- [ ] The change is scoped and unrelated work is preserved.
- [ ] Public API and XML documentation are complete.
- [ ] Tests cover success, boundary, failure, disposal, and cross-platform cases as relevant.
- [ ] Build, tests, format verification, and relevant samples/docs build pass.
- [ ] No blocking async code, terminal state leaks, or avoidable hot-path allocations were introduced.
- [ ] README and Diátaxis documents match the implementation.

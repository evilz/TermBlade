namespace TermBlade.Docs.Wasm.Terminal;

/// <summary>
/// Abstraction for terminal output. Allows the fake shell to write
/// to xterm.js via Blazor interop or to any other target (e.g. tests).
/// </summary>
public interface ITerminalOutput
{
    Task WriteAsync(string text);
    Task WriteLineAsync(string text);
    Task ClearAsync();
}

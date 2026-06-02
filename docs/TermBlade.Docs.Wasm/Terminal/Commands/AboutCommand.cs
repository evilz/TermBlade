namespace TermBlade.Docs.Wasm.Terminal.Commands;

public sealed class AboutCommand : ITerminalCommand
{
    public string Name => "about";
    public string Description => "Show information about TermBlade";

    public async Task ExecuteAsync(string[] args, ITerminalOutput output)
    {
        await output.WriteLineAsync("");
        await output.WriteLineAsync("\x1b[1;37m  🗡️ TermBlade\x1b[0m");
        await output.WriteLineAsync("  \x1b[2mTerminal UI core library for .NET\x1b[0m");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[36mFeatures:\x1b[0m");
        await output.WriteLineAsync("    • Cell-based rendering engine");
        await output.WriteLineAsync("    • ANSI escape sequence support");
        await output.WriteLineAsync("    • CSS Flexbox-like layout system");
        await output.WriteLineAsync("    • Styled text with rope-backed edit buffer");
        await output.WriteLineAsync("    • Full undo/redo support");
        await output.WriteLineAsync("    • 27+ UI components");
        await output.WriteLineAsync("    • Plugin architecture");
        await output.WriteLineAsync("    • Razor component wrappers");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[36mLinks:\x1b[0m");
        await output.WriteLineAsync("    \x1b[34mhttps://github.com/evilz/TermBlade\x1b[0m");
        await output.WriteLineAsync("    \x1b[34mhttps://www.nuget.org/packages/TermBlade.Core\x1b[0m");
        await output.WriteLineAsync("");
    }
}

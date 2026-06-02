using System.Text;

namespace TermBlade.Docs.Wasm.Terminal;

/// <summary>
/// Fake shell engine that processes keyboard input and dispatches commands.
/// No real OS access — purely a demo/documentation shell.
/// </summary>
public sealed class FakeShell
{
    private readonly Dictionary<string, ITerminalCommand> _commands = new(StringComparer.OrdinalIgnoreCase);
    private readonly StringBuilder _inputBuffer = new();
    private ITerminalOutput _output = null!;

    public string Prompt { get; set; } = "\x1b[38;2;0;200;100mtermblade\x1b[0m \x1b[38;2;100;100;255m❯\x1b[0m ";

    public void RegisterCommand(ITerminalCommand command)
    {
        _commands[command.Name] = command;
    }

    public void SetOutput(ITerminalOutput output)
    {
        _output = output;
    }

    public IReadOnlyDictionary<string, ITerminalCommand> Commands => _commands;

    /// <summary>
    /// Write the welcome banner and initial prompt.
    /// </summary>
    public async Task StartAsync()
    {
        await _output.WriteLineAsync("\x1b[1;38;2;0;200;100m");
        await _output.WriteLineAsync("  ╔══════════════════════════════════════════╗");
        await _output.WriteLineAsync("  ║         🗡️  TermBlade Interactive        ║");
        await _output.WriteLineAsync("  ║         Documentation Terminal           ║");
        await _output.WriteLineAsync("  ╚══════════════════════════════════════════╝\x1b[0m");
        await _output.WriteLineAsync("");
        await _output.WriteLineAsync("  \x1b[2mType \x1b[0m\x1b[1mhelp\x1b[0m\x1b[2m to see available commands.\x1b[0m");
        await _output.WriteLineAsync("");
        await _output.WriteAsync(Prompt);
    }

    /// <summary>
    /// Process raw terminal data from xterm.js key input.
    /// </summary>
    public async Task ProcessInputAsync(string data)
    {
        foreach (var ch in data)
        {
            switch (ch)
            {
                case '\r': // Enter
                case '\n':
                    await _output.WriteLineAsync("");
                    var line = _inputBuffer.ToString().Trim();
                    _inputBuffer.Clear();

                    if (!string.IsNullOrEmpty(line))
                    {
                        await ExecuteCommandAsync(line);
                    }

                    await _output.WriteAsync(Prompt);
                    break;

                case '\x7f': // Backspace
                case '\b':
                    if (_inputBuffer.Length > 0)
                    {
                        _inputBuffer.Remove(_inputBuffer.Length - 1, 1);
                        await _output.WriteAsync("\b \b");
                    }
                    break;

                case '\x03': // Ctrl+C
                    _inputBuffer.Clear();
                    await _output.WriteLineAsync("^C");
                    await _output.WriteAsync(Prompt);
                    break;

                default:
                    if (ch >= 32) // Printable
                    {
                        _inputBuffer.Append(ch);
                        await _output.WriteAsync(ch.ToString());
                    }
                    break;
            }
        }
    }

    private async Task ExecuteCommandAsync(string line)
    {
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var commandName = parts[0];
        var args = parts.Length > 1 ? parts[1..] : [];

        if (_commands.TryGetValue(commandName, out var command))
        {
            try
            {
                await command.ExecuteAsync(args, _output);
            }
            catch (Exception ex)
            {
                await _output.WriteLineAsync($"\x1b[31mError: {ex.Message}\x1b[0m");
            }
        }
        else
        {
            await _output.WriteLineAsync($"\x1b[31mUnknown command: {commandName}\x1b[0m");
            await _output.WriteLineAsync("\x1b[2mType 'help' for available commands.\x1b[0m");
        }
    }
}

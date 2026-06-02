namespace TermBlade.Docs.Wasm.Terminal.Commands;

public sealed class HelpCommand : ITerminalCommand
{
  private readonly FakeShell _shell;

  public HelpCommand(FakeShell shell) => _shell = shell;

  public string Name => "help";
  public string Description => "Show available commands";

  public async Task ExecuteAsync(string[] args, ITerminalOutput output)
  {
    await output.WriteLineAsync("\x1b[1;33m Available Commands:\x1b[0m");
    await output.WriteLineAsync("");

    foreach (var cmd in _shell.Commands.Values.OrderBy(c => c.Name))
    {
      await output.WriteLineAsync($"  \x1b[36m{cmd.Name,-16}\x1b[0m {cmd.Description}");
    }

    await output.WriteLineAsync("");
  }
}

namespace TermBlade.Docs.Wasm.Terminal.Commands;

public sealed class ClearCommand : ITerminalCommand
{
  public string Name => "clear";
  public string Description => "Clear the terminal screen";

  public async Task ExecuteAsync(string[] args, ITerminalOutput output)
  {
    await output.ClearAsync();
  }
}

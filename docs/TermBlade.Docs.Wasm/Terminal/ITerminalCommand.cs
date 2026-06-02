namespace TermBlade.Docs.Wasm.Terminal;

/// <summary>
/// A command that can be executed in the fake terminal shell.
/// </summary>
public interface ITerminalCommand
{
  /// <summary>Command name (e.g. "help").</summary>
  string Name { get; }

  /// <summary>Short description shown in help output.</summary>
  string Description { get; }

  /// <summary>Execute the command with the given arguments.</summary>
  Task ExecuteAsync(string[] args, ITerminalOutput output);
}

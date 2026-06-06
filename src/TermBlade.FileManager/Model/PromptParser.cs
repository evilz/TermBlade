namespace TermBlade.FileManager;

internal enum PromptMode
{
  None,
  Shell,
  Spf
}

internal enum SpfCommandKind
{
  Unknown,
  Split,
  Open,
  ChangeDirectory,
  MakeDirectory,
  Touch,
  Rename,
  Copy,
  Cut,
  Paste,
  Delete,
  Refresh
}

internal readonly record struct SpfCommand(SpfCommandKind Kind, string? Argument);

internal static class PromptParser
{
  public static SpfCommand ParseSpf(string input)
  {
    var trimmed = input.Trim();
    if (trimmed.Equals("split", StringComparison.OrdinalIgnoreCase))
      return new SpfCommand(SpfCommandKind.Split, null);

    var separator = trimmed.IndexOf(' ');
    var verb = separator < 0 ? trimmed : trimmed[..separator];
    var argument = separator < 0 ? null : trimmed[(separator + 1)..].Trim();

    return verb.ToLowerInvariant() switch
    {
      "open" when !string.IsNullOrWhiteSpace(argument) => new SpfCommand(SpfCommandKind.Open, argument),
      "cd" when !string.IsNullOrWhiteSpace(argument) => new SpfCommand(SpfCommandKind.ChangeDirectory, argument),
      "mkdir" when !string.IsNullOrWhiteSpace(argument) => new SpfCommand(SpfCommandKind.MakeDirectory, argument),
      "touch" when !string.IsNullOrWhiteSpace(argument) => new SpfCommand(SpfCommandKind.Touch, argument),
      "rename" when !string.IsNullOrWhiteSpace(argument) => new SpfCommand(SpfCommandKind.Rename, argument),
      "copy" => new SpfCommand(SpfCommandKind.Copy, null),
      "cut" => new SpfCommand(SpfCommandKind.Cut, null),
      "paste" => new SpfCommand(SpfCommandKind.Paste, argument),
      "delete" => new SpfCommand(SpfCommandKind.Delete, null),
      "refresh" => new SpfCommand(SpfCommandKind.Refresh, null),
      _ => new SpfCommand(SpfCommandKind.Unknown, argument)
    };
  }
}

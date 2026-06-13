namespace TermBlade.CsvViewer;

internal sealed record CsvViewerOptions(string? FilePath, char? Delimiter, bool HasHeader)
{
  public static CsvViewerOptions Parse(string[] args)
  {
    ArgumentNullException.ThrowIfNull(args);

    string? filePath = null;
    char? delimiter = null;
    var hasHeader = true;

    for (var i = 0; i < args.Length; i++)
    {
      switch (args[i])
      {
        case "--no-header":
          hasHeader = false;
          break;
        case "-d":
        case "--delimiter":
          if (i + 1 >= args.Length || string.IsNullOrEmpty(args[i + 1]))
            throw new ArgumentException("Delimiter option requires a value.");

          delimiter = ParseDelimiter(args[++i]);
          break;
        default:
          if (args[i].StartsWith('-', StringComparison.Ordinal))
            throw new ArgumentException($"Unknown option: {args[i]}");

          if (filePath is not null)
            throw new ArgumentException("Only one CSV file path can be specified.");

          filePath = args[i];
          break;
      }
    }

    return new CsvViewerOptions(filePath, delimiter, hasHeader);
  }

  private static char ParseDelimiter(string value)
  {
    return value.Equals("tab", StringComparison.OrdinalIgnoreCase) || value == @"\t"
        ? '\t'
        : value[0];
  }
}

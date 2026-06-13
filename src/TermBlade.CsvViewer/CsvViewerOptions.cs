namespace TermBlade.CsvViewer;

internal sealed record CsvViewerOptions(string? FilePath, char? Delimiter, bool HasHeader)
{
  public static CsvViewerOptions Parse(string[] args)
  {
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
          {
            throw new ArgumentException("Delimiter option requires a value.");
          }

          delimiter = args[++i][0];
          break;
        default:
          filePath ??= args[i];
          break;
      }
    }

    return new CsvViewerOptions(filePath, delimiter, hasHeader);
  }
}

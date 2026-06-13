namespace TermBlade.CsvViewer;

internal sealed class CsvViewerStartupOptions
{
  public string FilePath { get; set; } = string.Empty;

  public char? Delimiter { get; set; }

  public bool HasHeader { get; set; } = true;
}

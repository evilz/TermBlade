namespace TermBlade.CsvViewer;

internal sealed class CsvViewerStartupOptions
{
  /// <summary>
  /// Gets or sets the file path.
  /// </summary>
  public string FilePath { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the delimiter.
  /// </summary>
  public char? Delimiter { get; set; }

  /// <summary>
  /// Gets or sets the has header.
  /// </summary>
  public bool HasHeader { get; set; } = true;
}

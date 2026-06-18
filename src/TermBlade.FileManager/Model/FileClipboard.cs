namespace TermBlade.FileManager;

internal sealed class FileClipboard
{
  /// <summary>
  /// Gets or sets the source path.
  /// </summary>
  public string? SourcePath { get; private set; }
  /// <summary>
  /// Gets or sets the is cut.
  /// </summary>
  public bool IsCut { get; private set; }

  /// <summary>
  /// Gets the has value.
  /// </summary>
  public bool HasValue => SourcePath != null;

  /// <summary>
  /// Set.
  /// </summary>
  /// <param name="sourcePath">The sourcePath value.</param>
  /// <param name="cut">The cut value.</param>
  public void Set(string sourcePath, bool cut)
  {
    SourcePath = sourcePath;
    IsCut = cut;
  }

  /// <summary>
  /// Clear.
  /// </summary>
  public void Clear()
  {
    SourcePath = null;
    IsCut = false;
  }
}

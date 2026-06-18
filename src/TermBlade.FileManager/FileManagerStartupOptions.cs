namespace TermBlade.FileManager;

internal sealed class FileManagerStartupOptions
{
  /// <summary>
  /// Gets or sets the start path.
  /// </summary>
  public string StartPath { get; set; } = Environment.CurrentDirectory;
}

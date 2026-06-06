namespace TermBlade.FileManager;

internal sealed class FileManagerStartupOptions
{
  public string StartPath { get; set; } = Environment.CurrentDirectory;
}

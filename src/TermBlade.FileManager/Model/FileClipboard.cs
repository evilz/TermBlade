namespace TermBlade.FileManager;

internal sealed class FileClipboard
{
  public string? SourcePath { get; private set; }
  public bool IsCut { get; private set; }

  public bool HasValue => SourcePath != null;

  public void Set(string sourcePath, bool cut)
  {
    SourcePath = sourcePath;
    IsCut = cut;
  }

  public void Clear()
  {
    SourcePath = null;
    IsCut = false;
  }
}

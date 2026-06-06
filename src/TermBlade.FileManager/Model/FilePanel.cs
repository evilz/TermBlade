namespace TermBlade.FileManager;

internal sealed class FilePanel
{
  private const int WindowSize = 26;

  public string CurrentPath { get; private set; }
  public IReadOnlyList<FileManagerEntry> Entries { get; private set; } = [];
  public int SelectedIndex { get; private set; }
  public int WindowStart { get; private set; }

  public FilePanel(string currentPath)
  {
    CurrentPath = currentPath;
  }

  public FileManagerEntry? SelectedEntry
      => Entries.Count == 0 ? null : Entries[Math.Clamp(SelectedIndex, 0, Entries.Count - 1)];

  public void NavigateTo(string path, IFileSystemOperations fileSystem)
  {
    CurrentPath = path;
    SelectedIndex = 0;
    WindowStart = 0;
    Refresh(fileSystem);
  }

  public void Refresh(IFileSystemOperations fileSystem)
  {
    Entries = fileSystem.ListEntries(CurrentPath);
    SelectedIndex = Entries.Count == 0 ? 0 : Math.Clamp(SelectedIndex, 0, Entries.Count - 1);
    WindowStart = Entries.Count == 0 ? 0 : Math.Clamp(WindowStart, 0, Entries.Count - 1);
    EnsureSelectionVisible();
  }

  public void MoveSelection(int delta)
  {
    if (Entries.Count == 0)
      return;

    SelectedIndex = Math.Clamp(SelectedIndex + delta, 0, Entries.Count - 1);
    EnsureSelectionVisible();
  }

  private void EnsureSelectionVisible()
  {
    if (Entries.Count == 0)
    {
      WindowStart = 0;
      return;
    }

    if (SelectedIndex < WindowStart)
      WindowStart = SelectedIndex;
    else if (SelectedIndex >= WindowStart + WindowSize)
      WindowStart = SelectedIndex - WindowSize + 1;
  }
}

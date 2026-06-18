namespace TermBlade.FileManager;

internal sealed class FilePanel
{
  private const int WindowSize = 26;

  /// <summary>
  /// Gets or sets the current path.
  /// </summary>
  public string CurrentPath { get; private set; }
  /// <summary>
  /// Gets or sets the entries.
  /// </summary>
  public IReadOnlyList<FileManagerEntry> Entries { get; private set; } = [];
  /// <summary>
  /// Gets or sets the selected index.
  /// </summary>
  public int SelectedIndex { get; private set; }
  /// <summary>
  /// Gets or sets the window start.
  /// </summary>
  public int WindowStart { get; private set; }

  /// <summary>
  /// File panel.
  /// </summary>
  /// <param name="currentPath">The currentPath value.</param>
  public FilePanel(string currentPath)
  {
    CurrentPath = currentPath;
  }

  /// <summary>
  /// Member.
  /// </summary>
  public FileManagerEntry? SelectedEntry
      => Entries.Count == 0 ? null : Entries[Math.Clamp(SelectedIndex, 0, Entries.Count - 1)];

  /// <summary>
  /// Navigate to.
  /// </summary>
  /// <param name="path">The path value.</param>
  /// <param name="fileSystem">The fileSystem value.</param>
  public void NavigateTo(string path, IFileSystemOperations fileSystem)
  {
    CurrentPath = path;
    SelectedIndex = 0;
    WindowStart = 0;
    Refresh(fileSystem);
  }

  /// <summary>
  /// Refresh.
  /// </summary>
  /// <param name="fileSystem">The fileSystem value.</param>
  public void Refresh(IFileSystemOperations fileSystem)
  {
    Entries = fileSystem.ListEntries(CurrentPath);
    SelectedIndex = Entries.Count == 0 ? 0 : Math.Clamp(SelectedIndex, 0, Entries.Count - 1);
    WindowStart = Entries.Count == 0 ? 0 : Math.Clamp(WindowStart, 0, Entries.Count - 1);
    EnsureSelectionVisible();
  }

  /// <summary>
  /// Move selection.
  /// </summary>
  /// <param name="delta">The delta value.</param>
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

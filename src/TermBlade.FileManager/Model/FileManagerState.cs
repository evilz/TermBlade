using System.Diagnostics;

namespace TermBlade.FileManager;

internal enum FileManagerFocus
{
  File,
  Sidebar,
  Processes,
  Metadata,
  Prompt,
  Preview
}

internal sealed class FileManagerState
{
  private readonly IFileSystemOperations _fileSystem;

  public List<FilePanel> Panels { get; } = [];
  public IReadOnlyList<SidebarEntry> SidebarEntries { get; private set; } = [];
  public FileClipboard Clipboard { get; } = new();
  public int ActivePanelIndex { get; private set; }
  public int SelectedSidebarIndex { get; private set; }
  public string? StartupError { get; private set; }
  public string Status { get; private set; } = "Ready";
  public FileManagerFocus Focus { get; private set; } = FileManagerFocus.File;
  public PromptMode PromptMode { get; private set; }
  public string PromptText { get; private set; } = string.Empty;
  public bool PreviewVisible { get; private set; } = true;
  public int PreviewScrollX { get; private set; }
  public int PreviewScrollY { get; private set; }
  public bool FooterVisible { get; private set; } = true;
  public ConfirmationRequest? PendingConfirmation { get; private set; }

  public FilePanel ActivePanel => Panels[ActivePanelIndex];

  private FileManagerState(string startPath, IFileSystemOperations fileSystem, IReadOnlyList<SidebarEntry> sidebarEntries)
  {
    _fileSystem = fileSystem;
    SidebarEntries = sidebarEntries;
    Panels.Add(new FilePanel(startPath));
  }

  public static FileManagerState Create(
      string startPath,
      IFileSystemOperations fileSystem,
      IReadOnlyList<SidebarEntry>? sidebarEntries = null)
  {
    var state = new FileManagerState(startPath, fileSystem, sidebarEntries ?? BuildDefaultSidebarEntries(startPath));
    if (!fileSystem.DirectoryExists(startPath))
    {
      state.StartupError = $"Start path does not exist: {startPath}";
      state.Status = state.StartupError;
      return state;
    }

    state.RefreshActivePanel();
    return state;
  }

  public void RefreshActivePanel()
  {
    Try(() =>
    {
      ActivePanel.Refresh(_fileSystem);
      Status = $"Refreshed {ActivePanel.CurrentPath}";
    });
  }

  public void OpenSelected()
  {
    var entry = ActivePanel.SelectedEntry;
    if (entry == null)
      return;

    Try(() =>
    {
      if (entry.Value.IsDirectory)
        ActivePanel.NavigateTo(entry.Value.FullPath, _fileSystem);
      else
        _fileSystem.OpenFile(entry.Value.FullPath);

      Status = entry.Value.IsDirectory ? $"Opened {entry.Value.FullPath}" : $"Launched {entry.Value.Name}";
    });
  }

  public void NavigateParent()
  {
    Try(() =>
    {
      var parent = _fileSystem.GetParentPath(ActivePanel.CurrentPath);
      if (string.Equals(parent, ActivePanel.CurrentPath, StringComparison.Ordinal))
        return;

      if (_fileSystem.DirectoryExists(parent))
        ActivePanel.NavigateTo(parent, _fileSystem);
    });
  }

  public void MoveSelection(int delta) => ActivePanel.MoveSelection(delta);

  public void MoveSidebarSelection(int delta)
  {
    if (SidebarEntries.Count == 0)
      return;

    SelectedSidebarIndex = Math.Clamp(SelectedSidebarIndex + delta, 0, SidebarEntries.Count - 1);
  }

  public void ActivateSelectedSidebarEntry()
  {
    if (SidebarEntries.Count == 0)
      return;

    var entry = SidebarEntries[Math.Clamp(SelectedSidebarIndex, 0, SidebarEntries.Count - 1)];
    ChangeDirectory(entry.Path);
    Focus = FileManagerFocus.File;
  }

  public void SplitActivePanel()
  {
    var panel = new FilePanel(ActivePanel.CurrentPath);
    panel.Refresh(_fileSystem);
    Panels.Insert(ActivePanelIndex + 1, panel);
    ActivePanelIndex++;
    Status = $"Opened panel {ActivePanelIndex + 1}";
  }

  public void CloseActivePanel()
  {
    if (Panels.Count <= 1)
      return;

    Panels.RemoveAt(ActivePanelIndex);
    ActivePanelIndex = Math.Clamp(ActivePanelIndex, 0, Panels.Count - 1);
    Status = $"Closed panel {ActivePanelIndex + 1}";
  }

  public void FocusNextPanel()
  {
    ActivePanelIndex = (ActivePanelIndex + 1) % Panels.Count;
    Focus = FileManagerFocus.File;
  }

  public void FocusPreviousPanel()
  {
    ActivePanelIndex = (ActivePanelIndex - 1 + Panels.Count) % Panels.Count;
    Focus = FileManagerFocus.File;
  }

  public void TogglePreview() => PreviewVisible = !PreviewVisible;

  public void ToggleFooter() => FooterVisible = !FooterVisible;

  public void ToggleFocus(FileManagerFocus focus)
  {
    Focus = Focus == focus ? FileManagerFocus.File : focus;
  }

  public void OpenPrompt(PromptMode mode)
  {
    Focus = FileManagerFocus.Prompt;
    PromptMode = mode;
    PromptText = string.Empty;
  }

  public void ClosePrompt()
  {
    Focus = FileManagerFocus.File;
    PromptMode = PromptMode.None;
    PromptText = string.Empty;
  }

  public void AppendPromptChar(char value) => PromptText += value;

  public void BackspacePrompt()
  {
    if (PromptText.Length > 0)
      PromptText = PromptText[..^1];
  }

  public async Task SubmitPromptAsync()
  {
    var mode = PromptMode;
    var text = PromptText;
    ClosePrompt();

    if (mode == PromptMode.Spf)
    {
      ExecuteSpf(PromptParser.ParseSpf(text));
      return;
    }

    if (mode == PromptMode.Shell && !string.IsNullOrWhiteSpace(text))
      await ExecuteShellAsync(text).ConfigureAwait(false);
  }

  public void ExecuteSpf(SpfCommand command)
  {
    switch (command.Kind)
    {
      case SpfCommandKind.Split:
        SplitActivePanel();
        break;
      case SpfCommandKind.Open:
        OpenPanelAt(command.Argument);
        break;
      case SpfCommandKind.ChangeDirectory:
        ChangeDirectory(command.Argument);
        break;
      case SpfCommandKind.MakeDirectory:
        CreateDirectory(command.Argument ?? string.Empty);
        break;
      case SpfCommandKind.Touch:
        CreateFile(command.Argument ?? string.Empty);
        break;
      case SpfCommandKind.Rename:
        RenameSelected(command.Argument ?? string.Empty);
        break;
      case SpfCommandKind.Copy:
        CopySelected(cut: false);
        break;
      case SpfCommandKind.Cut:
        CopySelected(cut: true);
        break;
      case SpfCommandKind.Paste:
        PasteClipboard(command.Argument);
        break;
      case SpfCommandKind.Delete:
        RequestDeleteSelected();
        break;
      case SpfCommandKind.Refresh:
        RefreshActivePanel();
        break;
      default:
        Status = "Unknown SPF command";
        break;
    }
  }

  public void CreateDirectory(string name)
  {
    Try(() =>
    {
      _fileSystem.CreateDirectory(_fileSystem.Combine(ActivePanel.CurrentPath, name));
      RefreshActivePanel();
    });
  }

  public void CreateFile(string name)
  {
    Try(() =>
    {
      _fileSystem.CreateFile(_fileSystem.Combine(ActivePanel.CurrentPath, name));
      RefreshActivePanel();
    });
  }

  public void RenameSelected(string newName)
  {
    var entry = ActivePanel.SelectedEntry;
    if (entry == null || string.IsNullOrWhiteSpace(newName))
      return;

    Try(() =>
    {
      _fileSystem.Rename(entry.Value.FullPath, newName);
      RefreshActivePanel();
    });
  }

  public void CopySelected(bool cut)
  {
    var entry = ActivePanel.SelectedEntry;
    if (entry == null)
      return;

    Clipboard.Set(entry.Value.FullPath, cut);
    Status = $"{(cut ? "Cut" : "Copied")} {entry.Value.Name}";
  }

  public ConfirmationRequest? PasteClipboard(string? destinationName = null)
  {
    if (!Clipboard.HasValue || Clipboard.SourcePath == null)
      return null;

    var targetName = string.IsNullOrWhiteSpace(destinationName)
        ? _fileSystem.GetFileName(Clipboard.SourcePath)
        : destinationName;
    var destinationPath = _fileSystem.Combine(ActivePanel.CurrentPath, targetName);
    if (_fileSystem.FileExists(destinationPath) || _fileSystem.DirectoryExists(destinationPath))
    {
      PendingConfirmation = new ConfirmationRequest(
          ConfirmationKind.Overwrite,
          $"Overwrite {targetName}?",
          () => ExecutePaste(destinationPath, overwrite: true));
      return PendingConfirmation;
    }

    ExecutePaste(destinationPath, overwrite: false);
    return null;
  }

  public ConfirmationRequest? RequestDeleteSelected()
  {
    var entry = ActivePanel.SelectedEntry;
    if (entry == null)
      return null;

    PendingConfirmation = new ConfirmationRequest(
        ConfirmationKind.Delete,
        $"Delete {entry.Value.Name}?",
        () =>
        {
          _fileSystem.Delete(entry.Value.FullPath, recursive: entry.Value.IsDirectory);
          RefreshActivePanel();
        });
    return PendingConfirmation;
  }

  public void ConfirmPending()
  {
    var confirmation = PendingConfirmation;
    PendingConfirmation = null;
    confirmation?.Confirm();
  }

  public void CancelPending()
  {
    PendingConfirmation = null;
    Status = "Cancelled";
  }

  public string BuildPreviewContent(int maxLines, int maxChars)
  {
    var document = BuildPreviewDocument(maxChars);
    if (!document.IsText)
      return document.Message;

    var lines = document.Content.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n').Take(Math.Max(1, maxLines));
    return $"File: {ActivePanel.SelectedEntry?.Name}\n{string.Join('\n', lines)}";
  }

  public PreviewDocument BuildPreviewDocument(int maxChars)
  {
    var entry = ActivePanel.SelectedEntry;
    if (entry == null)
      return new PreviewDocument(false, string.Empty, string.Empty, 1, 1, "No file selected");

    if (entry.Value.IsDirectory)
      return new PreviewDocument(
          false,
          string.Empty,
          string.Empty,
          1,
          1,
          $"Directory\n{entry.Value.FullPath}\n{ActivePanel.Entries.Count} entries in current panel");

    var text = _fileSystem.ReadTextPreview(entry.Value.FullPath, maxChars);
    if (text == null)
      return new PreviewDocument(
          false,
          string.Empty,
          string.Empty,
          1,
          1,
          $"File\n{entry.Value.FullPath}\n{FormatSize(entry.Value.Size)}\nPreview unavailable for binary or unreadable files.");

    var normalized = text.Replace("\r\n", "\n").Replace('\r', '\n');
    var lines = normalized.Split('\n');
    var lineNumberWidth = lines.Length.ToString().Length + 1;
    return new PreviewDocument(
        true,
        normalized,
        DetectLanguage(entry.Value.FullPath),
        Math.Max(1, lines.Max(line => line.Length) + lineNumberWidth),
        Math.Max(1, lines.Length),
        string.Empty);
  }

  public void ScrollPreview(int deltaX, int deltaY, int viewportWidth, int viewportHeight)
  {
    var document = BuildPreviewDocument(maxChars: 64 * 1024);
    var maxX = Math.Max(0, document.ContentWidth - Math.Max(1, viewportWidth));
    var maxY = Math.Max(0, document.ContentHeight - Math.Max(1, viewportHeight));
    PreviewScrollX = Math.Clamp(PreviewScrollX + deltaX, 0, maxX);
    PreviewScrollY = Math.Clamp(PreviewScrollY + deltaY, 0, maxY);
  }

  private void OpenPanelAt(string? path)
  {
    var resolved = ResolvePath(path);
    if (resolved == null || !_fileSystem.DirectoryExists(resolved))
    {
      Status = $"Directory not found: {path}";
      return;
    }

    var panel = new FilePanel(resolved);
    panel.Refresh(_fileSystem);
    Panels.Add(panel);
    ActivePanelIndex = Panels.Count - 1;
  }

  private void ChangeDirectory(string? path)
  {
    var resolved = ResolvePath(path);
    if (resolved == null || !_fileSystem.DirectoryExists(resolved))
    {
      Status = $"Directory not found: {path}";
      return;
    }

    ActivePanel.NavigateTo(resolved, _fileSystem);
  }

  private string? ResolvePath(string? path)
  {
    if (string.IsNullOrWhiteSpace(path))
      return null;

    var expanded = PathExpander.Expand(path);
    return Path.IsPathRooted(expanded)
        ? Path.GetFullPath(expanded)
        : Path.GetFullPath(Path.Combine(ActivePanel.CurrentPath, expanded));
  }

  private void ExecutePaste(string destinationPath, bool overwrite)
  {
    if (Clipboard.SourcePath == null)
      return;

    Try(() =>
    {
      if (Clipboard.IsCut)
      {
        _fileSystem.Move(Clipboard.SourcePath, destinationPath, overwrite);
        Clipboard.Clear();
      }
      else
      {
        _fileSystem.Copy(Clipboard.SourcePath, destinationPath, overwrite);
      }

      RefreshActivePanel();
    });
  }

  private async Task ExecuteShellAsync(string command)
  {
    try
    {
      using var process = Process.Start(new ProcessStartInfo
      {
        FileName = OperatingSystem.IsWindows() ? "cmd.exe" : "/bin/sh",
        Arguments = OperatingSystem.IsWindows() ? $"/c {command}" : $"-c \"{command}\"",
        WorkingDirectory = ActivePanel.CurrentPath,
        UseShellExecute = false,
        CreateNoWindow = true
      });

      if (process == null)
      {
        Status = "Command failed to start";
        return;
      }

      await process.WaitForExitAsync().ConfigureAwait(false);
      Status = $"Exit code: {process.ExitCode}";
    }
    catch (Exception ex)
    {
      Status = ex.Message;
    }
  }

  private void Try(Action action)
  {
    try
    {
      action();
    }
    catch (Exception ex)
    {
      Status = ex.Message;
    }
  }

  private static string FormatSize(long size)
  {
    if (size < 1024)
      return $"{size} B";
    if (size < 1024 * 1024)
      return $"{size / 1024d:0.0} KB";

    return $"{size / 1024d / 1024d:0.0} MB";
  }

  private static string DetectLanguage(string path)
      => Path.GetExtension(path).ToLowerInvariant() switch
      {
        ".cs" or ".csx" => "csharp",
        ".js" or ".mjs" or ".cjs" => "javascript",
        ".ts" or ".tsx" => "typescript",
        ".json" => "json",
        ".html" or ".htm" => "html",
        ".css" => "css",
        ".xml" => "xml",
        ".md" or ".markdown" => "markdown",
        ".ps1" => "powershell",
        ".sh" or ".bash" or ".zsh" => "bash",
        ".yml" or ".yaml" => "yaml",
        ".py" => "python",
        ".rs" => "rust",
        ".go" => "go",
        ".java" => "java",
        ".cpp" or ".cc" or ".cxx" or ".hpp" or ".h" => "cpp",
        _ => string.Empty
      };

  private static IReadOnlyList<SidebarEntry> BuildDefaultSidebarEntries(string startPath)
  {
    var entries = new List<SidebarEntry>();
    AddSpecialFolder(entries, "Home", Environment.SpecialFolder.UserProfile);
    AddSpecialFolder(entries, "Downloads", Environment.SpecialFolder.UserProfile, "Downloads");
    AddSpecialFolder(entries, "Documents", Environment.SpecialFolder.MyDocuments);
    AddSpecialFolder(entries, "Pictures", Environment.SpecialFolder.MyPictures);
    AddSpecialFolder(entries, "Music", Environment.SpecialFolder.MyMusic);
    AddSpecialFolder(entries, "Desktop", Environment.SpecialFolder.Desktop);

    entries.Add(new SidebarEntry("TermBlade", startPath, SidebarEntryKind.Pinned));

    foreach (var drive in DriveInfo.GetDrives().Where(drive => drive.IsReady).Take(6))
      entries.Add(new SidebarEntry(drive.Name.TrimEnd(Path.DirectorySeparatorChar), drive.RootDirectory.FullName, SidebarEntryKind.Disk));

    return entries;
  }

  private static void AddSpecialFolder(List<SidebarEntry> entries, string label, Environment.SpecialFolder folder, string? child = null)
  {
    var path = Environment.GetFolderPath(folder);
    if (string.IsNullOrWhiteSpace(path))
      return;

    if (child != null)
      path = Path.Combine(path, child);

    if (Directory.Exists(path))
      entries.Add(new SidebarEntry(label, path, SidebarEntryKind.Home));
  }
}

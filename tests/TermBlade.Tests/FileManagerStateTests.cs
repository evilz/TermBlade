using TermBlade.FileManager;

namespace TermBlade.Tests;

public class FileManagerStateTests
{
  [Fact]
  public void Refresh_ClampsSelection_WhenEntriesShrink()
  {
    var fileSystem = new FakeFileSystem("/repo");
    fileSystem.SetEntries("/repo", [
        FileManagerEntry.Directory("/repo/src"),
        FileManagerEntry.File("/repo/README.md", 42),
        FileManagerEntry.File("/repo/LICENSE", 12)
    ]);
    var state = FileManagerState.Create("/repo", fileSystem);

    state.ActivePanel.MoveSelection(2);
    fileSystem.SetEntries("/repo", [FileManagerEntry.Directory("/repo/src")]);
    state.RefreshActivePanel();

    Assert.Equal(0, state.ActivePanel.SelectedIndex);
    Assert.Equal("src", state.ActivePanel.SelectedEntry?.Name);
  }

  [Fact]
  public void NavigateIntoSelectedDirectory_UpdatesActivePath()
  {
    var fileSystem = new FakeFileSystem("/repo");
    fileSystem.SetEntries("/repo", [FileManagerEntry.Directory("/repo/src")]);
    fileSystem.SetEntries("/repo/src", [FileManagerEntry.File("/repo/src/App.cs", 7)]);
    var state = FileManagerState.Create("/repo", fileSystem);

    state.OpenSelected();

    Assert.Equal("/repo/src", state.ActivePanel.CurrentPath);
    Assert.Equal("App.cs", state.ActivePanel.SelectedEntry?.Name);
  }

  [Fact]
  public void NavigateParent_StopsAtRoot()
  {
    var fileSystem = new FakeFileSystem("/");
    fileSystem.SetEntries("/", [FileManagerEntry.Directory("/repo")]);
    var state = FileManagerState.Create("/", fileSystem);

    state.NavigateParent();

    Assert.Equal("/", state.ActivePanel.CurrentPath);
  }

  [Fact]
  public void FilePanels_CanSplitCloseAndWrapFocus()
  {
    var state = FileManagerState.Create("/repo", new FakeFileSystem("/repo"));

    state.SplitActivePanel();
    state.SplitActivePanel();
    state.FocusNextPanel();
    state.FocusNextPanel();
    state.FocusNextPanel();
    state.FocusPreviousPanel();
    state.CloseActivePanel();

    Assert.Equal(2, state.Panels.Count);
    Assert.Equal(1, state.ActivePanelIndex);
  }

  [Fact]
  public void PasteCut_RequiresOverwriteConfirmation_WhenDestinationExists()
  {
    var fileSystem = new FakeFileSystem("/repo");
    fileSystem.SetEntries("/repo", [
        FileManagerEntry.File("/repo/source.txt", 1),
        FileManagerEntry.File("/repo/target.txt", 1)
    ]);
    var state = FileManagerState.Create("/repo", fileSystem);

    state.CopySelected(cut: true);
    state.ActivePanel.MoveSelection(1);
    var request = state.PasteClipboard(destinationName: "target.txt");

    Assert.NotNull(request);
    Assert.Equal(ConfirmationKind.Overwrite, request.Kind);
    Assert.False(fileSystem.Moved);
  }

  [Fact]
  public void DeleteSelected_RequiresConfirmation()
  {
    var fileSystem = new FakeFileSystem("/repo");
    fileSystem.SetEntries("/repo", [FileManagerEntry.File("/repo/source.txt", 1)]);
    var state = FileManagerState.Create("/repo", fileSystem);

    var request = state.RequestDeleteSelected();

    Assert.NotNull(request);
    Assert.Equal(ConfirmationKind.Delete, request.Kind);
    Assert.False(fileSystem.Deleted);
  }

  [Fact]
  public void BuildPreviewContent_ReadsTextFilePreview()
  {
    var fileSystem = new FakeFileSystem("/repo");
    fileSystem.SetEntries("/repo", [FileManagerEntry.File("/repo/notes.txt", 28)]);
    fileSystem.SetText("/repo/notes.txt", "first line\nsecond line\nthird line");
    var state = FileManagerState.Create("/repo", fileSystem);

    var preview = state.BuildPreviewContent(maxLines: 2, maxChars: 80);

    Assert.Contains("notes.txt", preview);
    Assert.Contains("first line", preview);
    Assert.Contains("second line", preview);
    Assert.DoesNotContain("third line", preview);
  }

  [Fact]
  public void BuildPreviewDocument_DetectsLanguageAndDimensions()
  {
    var fileSystem = new FakeFileSystem("/repo");
    fileSystem.SetEntries("/repo", [FileManagerEntry.File("/repo/Program.cs", 38)]);
    fileSystem.SetText("/repo/Program.cs", "using System;\nConsole.WriteLine(\"hello\");");
    var state = FileManagerState.Create("/repo", fileSystem);

    var preview = state.BuildPreviewDocument(maxChars: 1000);

    Assert.True(preview.IsText);
    Assert.Equal("csharp", preview.Language);
    Assert.Equal(2, preview.ContentHeight);
    Assert.True(preview.ContentWidth >= "Console.WriteLine(\"hello\");".Length);
  }

  [Fact]
  public void PreviewScroll_ClampsToDocumentBounds()
  {
    var fileSystem = new FakeFileSystem("/repo");
    fileSystem.SetEntries("/repo", [FileManagerEntry.File("/repo/notes.txt", 100)]);
    fileSystem.SetText("/repo/notes.txt", string.Join('\n', Enumerable.Range(1, 30).Select(i => $"line {i}")));
    var state = FileManagerState.Create("/repo", fileSystem);

    state.ScrollPreview(deltaX: 10, deltaY: 50, viewportWidth: 20, viewportHeight: 5);
    state.ScrollPreview(deltaX: -100, deltaY: -100, viewportWidth: 20, viewportHeight: 5);

    Assert.Equal(0, state.PreviewScrollX);
    Assert.Equal(0, state.PreviewScrollY);
  }

  [Fact]
  public void ActivateSelectedSidebarEntry_NavigatesActivePanel()
  {
    var fileSystem = new FakeFileSystem("/repo");
    var homePath = Path.GetFullPath(Path.Combine(Path.GetPathRoot(Environment.CurrentDirectory)!, "home", "user"));
    var todoPath = Path.Combine(homePath, "todo.txt");
    fileSystem.SetEntries("/repo", []);
    fileSystem.SetEntries(homePath, [FileManagerEntry.File(todoPath, 1)]);
    var state = FileManagerState.Create(
        "/repo",
        fileSystem,
        [new SidebarEntry("Home", homePath, SidebarEntryKind.Home)]);

    state.ToggleFocus(FileManagerFocus.Sidebar);
    state.ActivateSelectedSidebarEntry();

    Assert.Equal(homePath, state.ActivePanel.CurrentPath);
    Assert.Equal("todo.txt", state.ActivePanel.SelectedEntry?.Name);
  }

  [Fact]
  public void MoveSidebarSelection_ClampsToEntries()
  {
    var fileSystem = new FakeFileSystem("/repo");
    fileSystem.SetEntries("/repo", []);
    var state = FileManagerState.Create(
        "/repo",
        fileSystem,
        [
            new SidebarEntry("Home", "/home/user", SidebarEntryKind.Home),
            new SidebarEntry("Downloads", "/home/user/Downloads", SidebarEntryKind.Home)
        ]);

    state.MoveSidebarSelection(10);
    state.MoveSidebarSelection(-10);

    Assert.Equal(0, state.SelectedSidebarIndex);
  }

  [Fact]
  public void BuildPreviewContent_ReportsBinaryOrUnreadableFile()
  {
    var fileSystem = new FakeFileSystem("/repo");
    fileSystem.SetEntries("/repo", [FileManagerEntry.File("/repo/app.bin", 3)]);
    var state = FileManagerState.Create("/repo", fileSystem);

    var preview = state.BuildPreviewContent(maxLines: 3, maxChars: 80);

    Assert.Contains("Preview unavailable", preview);
  }

  [Theory]
  [InlineData("split", "Split", null)]
  [InlineData("open ~/src", "Open", "~/src")]
  [InlineData("cd ${HOME}", "ChangeDirectory", "${HOME}")]
  public void PromptParser_ParsesSpfCommands(string input, string kind, string? argument)
  {
    var command = PromptParser.ParseSpf(input);

    Assert.Equal(kind, command.Kind.ToString());
    Assert.Equal(argument, command.Argument);
  }

  [Theory]
  [InlineData("mkdir logs", "MakeDirectory", "logs")]
  [InlineData("touch notes.md", "Touch", "notes.md")]
  [InlineData("rename app.cs", "Rename", "app.cs")]
  [InlineData("copy", "Copy", null)]
  [InlineData("cut", "Cut", null)]
  [InlineData("paste backup.txt", "Paste", "backup.txt")]
  [InlineData("delete", "Delete", null)]
  [InlineData("refresh", "Refresh", null)]
  public void PromptParser_ParsesFileOperationCommands(string input, string kind, string? argument)
  {
    var command = PromptParser.ParseSpf(input);

    Assert.Equal(kind, command.Kind.ToString());
    Assert.Equal(argument, command.Argument);
  }

  [Fact]
  public void Create_InvalidStartPath_ReturnsErrorState()
  {
    var fileSystem = new FakeFileSystem("/repo");

    var state = FileManagerState.Create("/missing", fileSystem);

    Assert.NotNull(state.StartupError);
    Assert.Equal("/missing", state.ActivePanel.CurrentPath);
  }

  private sealed class FakeFileSystem(string rootPath) : IFileSystemOperations
  {
    private readonly Dictionary<string, IReadOnlyList<FileManagerEntry>> _entries = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _text = new(StringComparer.Ordinal);

    public bool Deleted { get; private set; }
    public bool Moved { get; private set; }

    public bool DirectoryExists(string path) => Normalize(path) == Normalize(rootPath) || _entries.ContainsKey(Normalize(path));

    public bool FileExists(string path)
        => _entries.Values.SelectMany(entries => entries).Any(entry => entry.FullPath == Normalize(path) && !entry.IsDirectory);

    public IReadOnlyList<FileManagerEntry> ListEntries(string path)
        => _entries.TryGetValue(Normalize(path), out var entries) ? entries : [];

    public string GetParentPath(string path)
    {
      var normalized = Normalize(path);
      if (normalized == "/" || normalized == Normalize(rootPath))
        return normalized == "/" ? "/" : "/";

      var index = normalized.LastIndexOf('/');
      return index <= 0 ? "/" : normalized[..index];
    }

    public string Combine(string path, string name) => Normalize(path.TrimEnd('/') + "/" + name);

    public string GetFileName(string path) => Normalize(path).Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? "/";

    public void OpenFile(string path)
    {
    }

    public void CreateDirectory(string path) => SetEntries(path, []);

    public void CreateFile(string path)
    {
    }

    public void Rename(string path, string newName)
    {
    }

    public void Copy(string sourcePath, string destinationPath, bool overwrite)
    {
    }

    public void Move(string sourcePath, string destinationPath, bool overwrite) => Moved = true;

    public void Delete(string path, bool recursive) => Deleted = true;

    public string? ReadTextPreview(string path, int maxChars)
        => _text.TryGetValue(Normalize(path), out var text) ? text[..Math.Min(text.Length, maxChars)] : null;

    public void SetEntries(string path, IReadOnlyList<FileManagerEntry> entries)
        => _entries[Normalize(path)] = entries;

    public void SetText(string path, string text) => _text[Normalize(path)] = text;

    private static string Normalize(string path) => path.Replace('\\', '/');
  }
}

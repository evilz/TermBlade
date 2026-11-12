using TermBlade.FileManager;

namespace TermBlade.Tests;

public sealed class SystemFileSystemOperationsTests
{
  [Fact]
  public void Move_ChildToParentOverwrite_ThrowsWithoutChangingTree()
  {
    using var workspace = new TemporaryWorkspace();
    var parentPath = workspace.CreateDirectory("parent");
    var childPath = workspace.CreateDirectory("parent/child");
    var uniquePath = workspace.CreateFile("parent/child/unique.txt", "data");
    var operations = new SystemFileSystemOperations();

    var error = Assert.Throws<IOException>(() => operations.Move(childPath, parentPath, overwrite: true));

    Assert.Contains("Cannot move a directory", error.Message, StringComparison.Ordinal);
    Assert.True(Directory.Exists(parentPath));
    Assert.True(Directory.Exists(childPath));
    Assert.True(File.Exists(uniquePath));
  }

  [Fact]
  public void Move_OverwriteMissingSource_PreservesExistingDestination()
  {
    using var workspace = new TemporaryWorkspace();
    var destinationPath = workspace.CreateDirectory("destination");
    var markerFile = workspace.CreateFile("destination/marker.txt", "keep me");
    var missingSourcePath = workspace.GetPath("missing-source");
    var operations = new SystemFileSystemOperations();

    Assert.Throws<FileNotFoundException>(() => operations.Move(missingSourcePath, destinationPath, overwrite: true));

    Assert.True(Directory.Exists(destinationPath));
    Assert.True(File.Exists(markerFile));
    Assert.Equal("keep me", File.ReadAllText(markerFile));
  }

  [Fact]
  public void PasteClipboard_CancelOverwrite_LeavesSourceAndDestinationUnchanged()
  {
    using var workspace = new TemporaryWorkspace();
    var sourcePath = workspace.CreateFile("source.txt", "source");
    var destinationPath = workspace.CreateFile("target.txt", "destination");
    var state = FileManagerState.Create(workspace.RootPath, new SystemFileSystemOperations());

    state.CopySelected(cut: true);
    var confirmation = state.PasteClipboard(destinationName: "target.txt");
    state.CancelPending();

    Assert.NotNull(confirmation);
    Assert.Equal(ConfirmationKind.Overwrite, confirmation.Kind);
    Assert.True(File.Exists(sourcePath));
    Assert.True(File.Exists(destinationPath));
    Assert.Equal("source", File.ReadAllText(sourcePath));
    Assert.Equal("destination", File.ReadAllText(destinationPath));
  }

  [Fact]
  public void Move_DirectoryAcrossVolumes_CopiesThenDeletesSource_WhenAlternateVolumeAvailable()
  {
    using var workspace = new TemporaryWorkspace();
    var sourcePath = workspace.CreateDirectory("source");
    _ = workspace.CreateFile("source/file.txt", "cross-volume");
    var alternateRoot = FindAlternateVolumeRoot(workspace.RootPath);
    if (alternateRoot == null)
      return;

    var destinationParent = Path.Combine(alternateRoot, "termblade-tests", Guid.NewGuid().ToString("N"));
    var destinationPath = Path.Combine(destinationParent, "moved-source");
    Directory.CreateDirectory(destinationParent);
    var operations = new SystemFileSystemOperations();

    try
    {
      operations.Move(sourcePath, destinationPath, overwrite: false);

      Assert.False(Directory.Exists(sourcePath));
      Assert.True(File.Exists(Path.Combine(destinationPath, "file.txt")));
    }
    finally
    {
      if (Directory.Exists(destinationParent))
        Directory.Delete(destinationParent, recursive: true);
    }
  }

  [Fact]
  public void Move_DirectoryWithDistinctCase_IsAllowedOnCaseSensitiveFilesystems()
  {
    if (OperatingSystem.IsWindows())
      return;

    using var workspace = new TemporaryWorkspace();
    var sourcePath = workspace.CreateDirectory("Project");
    if (Directory.Exists(workspace.GetPath("project")))
      return;

    var destinationRoot = workspace.CreateDirectory("project");
    var sourceFile = workspace.CreateFile("Project/marker.txt", "data");
    var destinationPath = Path.Combine(destinationRoot, "moved");
    var operations = new SystemFileSystemOperations();

    operations.Move(sourcePath, destinationPath, overwrite: false);

    Assert.False(Directory.Exists(sourcePath));
    Assert.True(File.Exists(Path.Combine(destinationPath, "marker.txt")));
    Assert.Equal("data", File.ReadAllText(Path.Combine(destinationPath, "marker.txt")));
  }

  [Fact]
  public void Move_DanglingSymbolicLink_UsesLinkEntryAsSource()
  {
    if (!OperatingSystem.IsLinux() && !OperatingSystem.IsMacOS())
      return;

    using var workspace = new TemporaryWorkspace();
    var sourcePath = workspace.GetPath("missing-target-link");
    var destinationPath = workspace.GetPath("moved-link");
    File.CreateSymbolicLink(sourcePath, workspace.GetPath("missing-target"));

    var operations = new SystemFileSystemOperations();
    operations.Move(sourcePath, destinationPath, overwrite: false);

    Assert.False(File.Exists(sourcePath));
    Assert.True(File.Exists(destinationPath));
    Assert.True((File.GetAttributes(destinationPath) & FileAttributes.ReparsePoint) != 0);
  }

  [Fact]
  public void CopyDirectory_PreservesExternalDirectoryLinkWithoutCopyingTarget()
  {
    if (OperatingSystem.IsWindows())
      return;

    using var workspace = new TemporaryWorkspace();
    var sourcePath = workspace.CreateDirectory("source");
    var outsidePath = workspace.CreateDirectory("outside");
    var markerPath = workspace.CreateFile("outside/private.txt", "private");
    Directory.CreateSymbolicLink(Path.Combine(sourcePath, "outside-link"), outsidePath);
    var destinationPath = workspace.GetPath("destination");
    var operations = new SystemFileSystemOperations();

    operations.Copy(sourcePath, destinationPath, overwrite: false);

    var copiedLink = new DirectoryInfo(Path.Combine(destinationPath, "outside-link"));
    Assert.NotNull(copiedLink.LinkTarget);
    Assert.True((copiedLink.Attributes & FileAttributes.ReparsePoint) != 0);
    Assert.Single(Directory.EnumerateFileSystemEntries(destinationPath));
    Assert.True(File.Exists(markerPath));
  }

  [Fact]
  public void Copy_RootDirectoryLinkPreservesLinkInsteadOfTraversingTarget()
  {
    if (OperatingSystem.IsWindows())
      return;

    using var workspace = new TemporaryWorkspace();
    var sourcePath = workspace.CreateDirectory("source");
    _ = workspace.CreateFile("source/marker.txt", "data");
    var linkPath = workspace.GetPath("source-link");
    var destinationPath = workspace.GetPath("destination-link");
    Directory.CreateSymbolicLink(linkPath, sourcePath);
    var operations = new SystemFileSystemOperations();

    operations.Copy(linkPath, destinationPath, overwrite: false);

    var copiedLink = new DirectoryInfo(destinationPath);
    Assert.NotNull(copiedLink.LinkTarget);
    Assert.True((copiedLink.Attributes & FileAttributes.ReparsePoint) != 0);
  }

  [Fact]
  public void Copy_DanglingSymbolicLinkPreservesLinkTarget()
  {
    if (OperatingSystem.IsWindows())
      return;

    using var workspace = new TemporaryWorkspace();
    var sourcePath = workspace.GetPath("dangling-link");
    var missingTarget = workspace.GetPath("missing-target");
    var destinationPath = workspace.GetPath("copied-link");
    File.CreateSymbolicLink(sourcePath, missingTarget);
    var operations = new SystemFileSystemOperations();

    operations.Copy(sourcePath, destinationPath, overwrite: false);

    var copiedLink = new FileInfo(destinationPath);
    Assert.NotNull(copiedLink.LinkTarget);
    Assert.Equal(Path.GetFullPath(missingTarget), Path.GetFullPath(copiedLink.LinkTarget!));
    Assert.True((File.GetAttributes(destinationPath) & FileAttributes.ReparsePoint) != 0);
  }

  [Fact]
  public void CopyDirectory_PreservesSelfAndMultiDirectoryLinksWithoutRecursing()
  {
    if (OperatingSystem.IsWindows())
      return;

    using var workspace = new TemporaryWorkspace();
    var sourcePath = workspace.CreateDirectory("source");
    var firstDirectory = workspace.CreateDirectory("source/first");
    var secondDirectory = workspace.CreateDirectory("source/second");
    Directory.CreateSymbolicLink(Path.Combine(sourcePath, "self"), sourcePath);
    Directory.CreateSymbolicLink(Path.Combine(firstDirectory, "to-second"), secondDirectory);
    Directory.CreateSymbolicLink(Path.Combine(secondDirectory, "to-first"), firstDirectory);
    var destinationPath = workspace.GetPath("destination");
    var operations = new SystemFileSystemOperations();

    operations.Copy(sourcePath, destinationPath, overwrite: false);

    var copiedSelf = new DirectoryInfo(Path.Combine(destinationPath, "self"));
    var copiedToSecond = new DirectoryInfo(Path.Combine(destinationPath, "first", "to-second"));
    var copiedToFirst = new DirectoryInfo(Path.Combine(destinationPath, "second", "to-first"));
    Assert.NotNull(copiedSelf.LinkTarget);
    Assert.NotNull(copiedToSecond.LinkTarget);
    Assert.NotNull(copiedToFirst.LinkTarget);
    Assert.Equal(3, Directory.EnumerateFileSystemEntries(destinationPath).Count());
    Assert.Single(Directory.EnumerateFileSystemEntries(Path.Combine(destinationPath, "first")));
    Assert.Single(Directory.EnumerateFileSystemEntries(Path.Combine(destinationPath, "second")));
  }

  private static string? FindAlternateVolumeRoot(string path)
  {
    var currentRoot = Path.GetPathRoot(Path.GetFullPath(path));
    if (!OperatingSystem.IsWindows() || currentRoot == null)
      return null;

    foreach (var drive in DriveInfo.GetDrives())
    {
      if (!drive.IsReady)
        continue;

      var candidateRoot = drive.RootDirectory.FullName;
      if (string.Equals(candidateRoot, currentRoot, StringComparison.OrdinalIgnoreCase))
        continue;

      var probe = Path.Combine(candidateRoot, "termblade-tests", Guid.NewGuid().ToString("N"));
      try
      {
        Directory.CreateDirectory(probe);
        Directory.Delete(probe);
        return candidateRoot;
      }
      catch
      {
        // Skip unwritable alternate roots.
      }
    }

    return null;
  }

  private sealed class TemporaryWorkspace : IDisposable
  {
    public TemporaryWorkspace()
    {
      RootPath = Path.Combine(Path.GetTempPath(), "termblade-tests", Guid.NewGuid().ToString("N"));
      Directory.CreateDirectory(RootPath);
    }

    public string RootPath { get; }

    public string GetPath(string relativePath)
        => Path.Combine(RootPath, relativePath.Replace('/', Path.DirectorySeparatorChar));

    public string CreateDirectory(string relativePath)
    {
      var path = GetPath(relativePath);
      Directory.CreateDirectory(path);
      return path;
    }

    public string CreateFile(string relativePath, string content)
    {
      var path = GetPath(relativePath);
      Directory.CreateDirectory(Path.GetDirectoryName(path)!);
      File.WriteAllText(path, content);
      return path;
    }

    public void Dispose()
    {
      if (Directory.Exists(RootPath))
        Directory.Delete(RootPath, recursive: true);
    }
  }
}

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
      if (!string.Equals(candidateRoot, currentRoot, StringComparison.OrdinalIgnoreCase))
        return candidateRoot;
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

using System.Diagnostics;

namespace TermBlade.FileManager;

internal sealed class SystemFileSystemOperations : IFileSystemOperations
{
  /// <summary>
  /// Gets the directory exists.
  /// </summary>
  public bool DirectoryExists(string path) => Directory.Exists(path);

  /// <summary>
  /// Gets the file exists.
  /// </summary>
  public bool FileExists(string path) => File.Exists(path);

  /// <summary>
  /// List entries.
  /// </summary>
  /// <param name="path">The path value.</param>
  public IReadOnlyList<FileManagerEntry> ListEntries(string path)
  {
    var directory = new DirectoryInfo(path);
    if (!directory.Exists)
      return [];

    return directory.EnumerateFileSystemInfos()
        .OrderByDescending(info => info is DirectoryInfo)
        .ThenBy(info => info.Name, StringComparer.CurrentCultureIgnoreCase)
        .Select(ToEntry)
        .ToList();
  }

  /// <summary>
  /// Get parent path.
  /// </summary>
  /// <param name="path">The path value.</param>
  public string GetParentPath(string path)
      => Directory.GetParent(path)?.FullName ?? Path.GetFullPath(path);

  /// <summary>
  /// Gets the combine.
  /// </summary>
  /// <param name="path">The path value.</param>
  public string Combine(string path, string name) => Path.GetFullPath(Path.Combine(path, name));

  /// <summary>
  /// Get file name.
  /// </summary>
  /// <param name="path">The path value.</param>
  public string GetFileName(string path)
  {
    var trimmed = path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    return Path.GetFileName(trimmed);
  }

  /// <summary>
  /// Open file.
  /// </summary>
  /// <param name="path">The path value.</param>
  public void OpenFile(string path)
  {
    if (!File.Exists(path))
      return;

    using var _ = Process.Start(new ProcessStartInfo
    {
      FileName = path,
      UseShellExecute = true
    });
  }

  /// <summary>
  /// Gets the create directory.
  /// </summary>
  public void CreateDirectory(string path) => Directory.CreateDirectory(path);

  /// <summary>
  /// Create file.
  /// </summary>
  /// <param name="path">The path value.</param>
  public void CreateFile(string path)
  {
    if (File.Exists(path))
    {
      File.SetLastWriteTime(path, DateTime.Now);
      return;
    }

    using var _ = File.Create(path);
  }

  /// <summary>
  /// Rename.
  /// </summary>
  /// <param name="path">The path value.</param>
  /// <param name="newName">The newName value.</param>
  public void Rename(string path, string newName)
  {
    var parent = Directory.GetParent(path)?.FullName ?? Environment.CurrentDirectory;
    var destination = Path.Combine(parent, newName);
    if (PathsEqual(path, destination))
    {
      TryMoveCaseOnly(path, destination);
      return;
    }

    if (Directory.Exists(path))
      Directory.Move(path, destination);
    else
      File.Move(path, destination);
  }

  /// <summary>
  /// Copy.
  /// </summary>
  /// <param name="sourcePath">The sourcePath value.</param>
  /// <param name="destinationPath">The destinationPath value.</param>
  /// <param name="overwrite">The overwrite value.</param>
  public void Copy(string sourcePath, string destinationPath, bool overwrite)
  {
    if (IsSymbolicLink(sourcePath))
    {
      CopyLink(sourcePath, destinationPath, overwrite);
      return;
    }

    if (Directory.Exists(sourcePath))
    {
      if (FileSystemPathComparison.IsSameOrDescendant(sourcePath, destinationPath))
        throw new IOException("Cannot copy a directory into itself or its subdirectory.");

      CopyDirectory(sourcePath, destinationPath, overwrite);
      return;
    }

    File.Copy(sourcePath, destinationPath, overwrite);
  }

  /// <summary>
  /// Move.
  /// </summary>
  /// <param name="sourcePath">The sourcePath value.</param>
  /// <param name="destinationPath">The destinationPath value.</param>
  /// <param name="overwrite">The overwrite value.</param>
  public void Move(string sourcePath, string destinationPath, bool overwrite)
  {
    if (PathsEqual(sourcePath, destinationPath))
    {
      TryMoveCaseOnly(sourcePath, destinationPath);
      return;
    }

    var sourceIsDirectory = IsDirectoryPath(sourcePath);
    var sourceIsFile = IsFilePath(sourcePath);
    if (!sourceIsDirectory && !sourceIsFile)
      throw new FileNotFoundException($"Source path not found: {sourcePath}", sourcePath);

    if (sourceIsDirectory)
      ValidateDirectoryMoveRelationship(sourcePath, destinationPath);

    if (!overwrite)
    {
      MoveWithoutOverwrite(sourcePath, destinationPath, sourceIsDirectory);
      return;
    }

    var destinationExists = IsExistingPath(destinationPath);
    if (!destinationExists)
    {
      MoveWithoutOverwrite(sourcePath, destinationPath, sourceIsDirectory);
      return;
    }

    var backupPath = BuildOverwriteBackupPath(destinationPath);
    var destinationIsDirectory = IsDirectoryPath(destinationPath);
    MoveExistingPath(destinationPath, backupPath, destinationIsDirectory);

    try
    {
      MoveWithoutOverwrite(sourcePath, destinationPath, sourceIsDirectory);
    }
    catch
    {
      Delete(destinationPath, recursive: true);
      MoveExistingPath(backupPath, destinationPath, destinationIsDirectory);
      throw;
    }

    Delete(backupPath, recursive: true);
  }

  /// <summary>
  /// Delete.
  /// </summary>
  /// <param name="path">The path value.</param>
  /// <param name="recursive">The recursive value.</param>
  public void Delete(string path, bool recursive)
      => DeletePath(path, recursive);

  private static void DeletePath(string path, bool recursive)
  {
    if (Directory.Exists(path) && !IsReparsePoint(path))
      Directory.Delete(path, recursive);
    else if (File.Exists(path) || IsReparsePoint(path))
      File.Delete(path);
  }

  /// <summary>
  /// Read text preview.
  /// </summary>
  /// <param name="path">The path value.</param>
  /// <param name="maxChars">The maxChars value.</param>
  public string? ReadTextPreview(string path, int maxChars)
  {
    if (!File.Exists(path) || maxChars <= 0)
      return null;

    try
    {
      using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      var buffer = new byte[Math.Max(1, maxChars * 4)];
      var read = stream.Read(buffer, 0, buffer.Length);
      if (read == 0)
        return string.Empty;

      if (buffer.AsSpan(0, read).Contains((byte)0))
        return null;

      using var reader = new StreamReader(new MemoryStream(buffer, 0, read), detectEncodingFromByteOrderMarks: true);
      var text = reader.ReadToEnd();
      return text.Length <= maxChars ? text : text[..maxChars];
    }
    catch
    {
      return null;
    }
  }

  private static FileManagerEntry ToEntry(FileSystemInfo info)
      => new(
          info.Name,
          info.FullName,
          info is DirectoryInfo,
          info is FileInfo file ? file.Length : 0,
          info.LastWriteTime,
          info is DirectoryInfo ? "drwxrwxrwx" : "-rw-rw-rw-",
          Environment.UserName,
          string.Empty);

  private static void CopyDirectory(string sourcePath, string destinationPath, bool overwrite)
  {
    Directory.CreateDirectory(destinationPath);
    RestoreDirectoryMetadata(sourcePath, destinationPath);

    foreach (var entry in new DirectoryInfo(sourcePath).EnumerateFileSystemInfos())
    {
      var destinationEntry = Path.Combine(destinationPath, entry.Name);
      if (IsReparsePoint(entry.FullName))
      {
        CopyLink(entry.FullName, destinationEntry, overwrite);
        continue;
      }

      if (entry is DirectoryInfo directory)
      {
        CopyDirectory(directory.FullName, destinationEntry, overwrite);
        continue;
      }

      var file = (FileInfo)entry;
      if (File.Exists(destinationEntry) && overwrite)
        File.Delete(destinationEntry);

      File.Copy(file.FullName, destinationEntry, overwrite);
      RestoreFileMetadata(file.FullName, destinationEntry);
    }
  }

  private static void RestoreDirectoryMetadata(string sourcePath, string destinationPath)
  {
    try
    {
      var sourceInfo = new DirectoryInfo(sourcePath);
      var attributes = sourceInfo.Attributes & ~FileAttributes.ReparsePoint;
      File.SetAttributes(destinationPath, attributes);
      new DirectoryInfo(destinationPath).LastWriteTimeUtc = sourceInfo.LastWriteTimeUtc;
    }
    catch
    {
      // Preserve the source tree without failing a move when metadata is unavailable.
    }
  }

  private static void RestoreFileMetadata(string sourcePath, string destinationPath)
  {
    try
    {
      var sourceInfo = new FileInfo(sourcePath);
      File.SetAttributes(destinationPath, sourceInfo.Attributes & ~FileAttributes.ReparsePoint);
      new FileInfo(destinationPath).LastWriteTimeUtc = sourceInfo.LastWriteTimeUtc;
    }
    catch
    {
      // Preserve the source data without failing a copy when metadata is unavailable.
    }
  }

  private static void CopyLink(string sourcePath, string destinationPath, bool overwrite)
  {
    var destinationExists = IsExistingPath(destinationPath);
    if (destinationExists && !overwrite)
      throw new IOException($"Destination path already exists: {destinationPath}");

    if (destinationExists && overwrite)
      DeletePath(destinationPath, recursive: true);

    var linkTarget = GetLinkTarget(sourcePath);
    var targetPath = Path.IsPathRooted(linkTarget)
      ? linkTarget
      : Path.Combine(Path.GetDirectoryName(sourcePath) ?? string.Empty, linkTarget);
    var shouldCreateDirectoryLink = (File.GetAttributes(sourcePath) & FileAttributes.Directory) != 0 || Directory.Exists(targetPath);
    if (shouldCreateDirectoryLink)
      Directory.CreateSymbolicLink(destinationPath, targetPath);
    else
      File.CreateSymbolicLink(destinationPath, targetPath);
  }

  private static void ValidateDirectoryMoveRelationship(string sourcePath, string destinationPath)
  {
    if (FileSystemPathComparison.IsSameOrDescendant(sourcePath, destinationPath)
        || FileSystemPathComparison.IsSameOrDescendant(destinationPath, sourcePath))
      throw new IOException("Cannot move a directory to itself, a parent directory, or a subdirectory.");
  }

  private static string BuildOverwriteBackupPath(string destinationPath)
  {
    var parent = Directory.GetParent(destinationPath)?.FullName ?? Path.GetPathRoot(Path.GetFullPath(destinationPath))!;
    var name = Path.GetFileName(destinationPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
    return Path.Combine(parent, $"{name}.termblade-overwrite-backup-{Guid.NewGuid():N}");
  }

  private static string BuildCaseOnlyRenameTemporaryPath(string sourcePath)
  {
    var parent = Path.GetDirectoryName(sourcePath) ?? Path.GetPathRoot(Path.GetFullPath(sourcePath))!;
    string temporaryPath;
    do
    {
      temporaryPath = Path.Combine(parent, $".termblade-rename-{Guid.NewGuid():N}");
    }
    while (IsExistingPath(temporaryPath));

    return temporaryPath;
  }

  private static bool TryMoveCaseOnly(string sourcePath, string destinationPath)
  {
    var sourceFullPath = Path.GetFullPath(sourcePath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    var destinationFullPath = Path.GetFullPath(destinationPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    if (string.Equals(sourceFullPath, destinationFullPath, StringComparison.Ordinal))
      return false;

    var sourceParent = Path.GetDirectoryName(sourceFullPath);
    var destinationParent = Path.GetDirectoryName(destinationFullPath);
    if (sourceParent is null
        || destinationParent is null
        || !PathsEqual(sourceParent, destinationParent))
      return false;

    var sourceName = Path.GetFileName(sourceFullPath);
    var destinationName = Path.GetFileName(destinationFullPath);
    if (string.Equals(sourceName, destinationName, StringComparison.Ordinal)
        || !string.Equals(sourceName, destinationName, StringComparison.OrdinalIgnoreCase))
      return false;

    var sourceIsDirectory = IsDirectoryPath(sourceFullPath);
    if (!sourceIsDirectory && !IsFilePath(sourceFullPath))
      return false;

    var temporaryPath = BuildCaseOnlyRenameTemporaryPath(sourceFullPath);
    MoveExistingPath(sourceFullPath, temporaryPath, sourceIsDirectory);
    try
    {
      MoveExistingPath(temporaryPath, destinationFullPath, sourceIsDirectory);
    }
    catch (Exception moveError)
    {
      try
      {
        MoveExistingPath(temporaryPath, sourceFullPath, sourceIsDirectory);
      }
      catch (Exception restoreError)
      {
        throw new IOException(
            $"Case-only rename failed and the original path could not be restored. Temporary path: {temporaryPath}",
            new AggregateException(moveError, restoreError));
      }

      throw;
    }

    return true;
  }

  private static void MoveExistingPath(string sourcePath, string destinationPath, bool sourceIsDirectory)
  {
    if (sourceIsDirectory)
      Directory.Move(sourcePath, destinationPath);
    else
      File.Move(sourcePath, destinationPath);
  }

  private static void MoveWithoutOverwrite(string sourcePath, string destinationPath, bool sourceIsDirectory)
  {
    if (sourceIsDirectory)
    {
      try
      {
        Directory.Move(sourcePath, destinationPath);
      }
      catch (IOException ex) when (!Directory.Exists(destinationPath) && IsCrossVolumeMove(ex, sourcePath, destinationPath))
      {
        try
        {
          CopyDirectory(sourcePath, destinationPath, overwrite: false);
          Directory.Delete(sourcePath, recursive: true);
        }
        catch
        {
          if (Directory.Exists(destinationPath))
            Directory.Delete(destinationPath, recursive: true);

          throw;
        }
      }

      return;
    }

    try
    {
      File.Move(sourcePath, destinationPath);
    }
    catch (IOException ex) when (!File.Exists(destinationPath) && IsCrossVolumeMove(ex, sourcePath, destinationPath))
    {
      try
      {
        File.Copy(sourcePath, destinationPath, overwrite: false);
        File.Delete(sourcePath);
      }
      catch
      {
        if (Directory.Exists(destinationPath) || File.Exists(destinationPath) || IsReparsePoint(destinationPath))
          DeletePath(destinationPath, recursive: true);

        throw;
      }
    }
  }

  private static bool IsCrossVolumeMove(IOException error, string sourcePath, string destinationPath)
  {
    const int errorNotSameDevice = 17; // EXDEV

    var sourceRoot = Path.GetPathRoot(Path.GetFullPath(sourcePath));
    var destinationRoot = Path.GetPathRoot(Path.GetFullPath(destinationPath));
    if (!string.Equals(sourceRoot, destinationRoot, GetRootPathComparison()))
      return true;

    // ERROR_NOT_SAME_DEVICE (0x80070011) and ERROR_LOCK_VIOLATION (0x80070021) are
    // the Win32-style values associated with cross-volume move failures.
    if (error.HResult == unchecked((int)0x80070011) || error.HResult == unchecked((int)0x80070021))
      return true;

    if (error.InnerException is System.ComponentModel.Win32Exception win32Exception)
      return win32Exception.NativeErrorCode == errorNotSameDevice;

    return false;
  }

  private static bool IsDirectoryPath(string path)
    => Directory.Exists(path) && !IsReparsePoint(path);

  private static bool IsFilePath(string path)
    => File.Exists(path) || IsSymbolicLink(path);

  private static bool IsExistingPath(string path)
    => Directory.Exists(path) || File.Exists(path) || IsSymbolicLink(path);

  private static bool IsReparsePoint(string path)
  {
    try
    {
      return (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0;
    }
    catch
    {
      return false;
    }
  }

  private static bool IsSymbolicLink(string path)
  {
    try
    {
      return (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0
          && new FileInfo(path).LinkTarget is not null;
    }
    catch
    {
      return false;
    }
  }

  private static string GetLinkTarget(string path)
  {
    var fileTarget = new FileInfo(path).LinkTarget;
    if (!string.IsNullOrEmpty(fileTarget))
      return fileTarget;

    var directoryTarget = new DirectoryInfo(path).LinkTarget;
    if (!string.IsNullOrEmpty(directoryTarget))
      return directoryTarget;

    throw new IOException($"The path is not a valid symbolic link: {path}");
  }

  private static StringComparison GetRootPathComparison()
    => OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

  private static bool PathsEqual(string path1, string path2)
      => FileSystemPathComparison.PathsEqual(path1, path2);
}

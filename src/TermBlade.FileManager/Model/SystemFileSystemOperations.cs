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
      var sourceReal = NormalizeDirectoryPath(sourcePath);
      var destinationReal = NormalizeDirectoryPath(destinationPath);
      if (destinationReal.StartsWith(sourceReal, GetPathComparison()))
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
      return;

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

    foreach (var entry in Directory.EnumerateFileSystemInfos(sourcePath))
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
      Directory.SetAttributes(destinationPath, attributes);
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
      Delete(destinationPath, recursive: true);

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

  private static string NormalizeDirectoryPath(string path)
      => Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;

  private static void ValidateDirectoryMoveRelationship(string sourcePath, string destinationPath)
  {
    var sourceReal = NormalizeDirectoryPath(sourcePath);
    var destinationReal = NormalizeDirectoryPath(destinationPath);
    if (sourceReal.StartsWith(destinationReal, GetPathComparison())
        || destinationReal.StartsWith(sourceReal, GetPathComparison()))
      throw new IOException("Cannot move a directory to itself, a parent directory, or a subdirectory.");
  }

  private static string BuildOverwriteBackupPath(string destinationPath)
  {
    var parent = Directory.GetParent(destinationPath)?.FullName ?? Path.GetPathRoot(Path.GetFullPath(destinationPath))!;
    var name = Path.GetFileName(destinationPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
    return Path.Combine(parent, $"{name}.termblade-overwrite-backup-{Guid.NewGuid():N}");
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
          Delete(destinationPath, recursive: true);

        throw;
      }
    }
  }

  private static bool IsCrossVolumeMove(IOException error, string sourcePath, string destinationPath)
  {
    const int errorNotSameDevice = 17; // EXDEV

    var sourceRoot = Path.GetPathRoot(Path.GetFullPath(sourcePath));
    var destinationRoot = Path.GetPathRoot(Path.GetFullPath(destinationPath));
    if (!string.Equals(sourceRoot, destinationRoot, GetPathComparison()))
      return true;

    // ERROR_NOT_SAME_DEVICE (0x80070011) and ERROR_LOCK_VIOLATION (0x80070021) are
    // the Win32-style values associated with cross-volume move failures.
    if (error.HResult == unchecked((int)0x80070011) || error.HResult == unchecked((int)0x80070021))
      return true;

    if (error is System.ComponentModel.Win32Exception win32Exception)
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

  private static StringComparison GetPathComparison()
    => OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

  private static bool PathsEqual(string path1, string path2)
  {
    var normalizedPath1 = Path.GetFullPath(path1).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    var normalizedPath2 = Path.GetFullPath(path2).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    return string.Equals(normalizedPath1, normalizedPath2, GetPathComparison());
  }
}

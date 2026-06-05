using System.Diagnostics;

namespace TermBlade.FileManager;

internal sealed class SystemFileSystemOperations : IFileSystemOperations
{
  public bool DirectoryExists(string path) => Directory.Exists(path);

  public bool FileExists(string path) => File.Exists(path);

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

  public string GetParentPath(string path)
      => Directory.GetParent(path)?.FullName ?? Path.GetFullPath(path);

  public string Combine(string path, string name) => Path.GetFullPath(Path.Combine(path, name));

  public string GetFileName(string path)
  {
    var trimmed = path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    return Path.GetFileName(trimmed);
  }

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

  public void CreateDirectory(string path) => Directory.CreateDirectory(path);

  public void CreateFile(string path)
  {
    using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
  }

  public void Rename(string path, string newName)
  {
    var parent = Directory.GetParent(path)?.FullName ?? Environment.CurrentDirectory;
    var destination = Path.Combine(parent, newName);
    if (Directory.Exists(path))
      Directory.Move(path, destination);
    else
      File.Move(path, destination);
  }

  public void Copy(string sourcePath, string destinationPath, bool overwrite)
  {
    if (Directory.Exists(sourcePath))
    {
      CopyDirectory(sourcePath, destinationPath, overwrite);
      return;
    }

    File.Copy(sourcePath, destinationPath, overwrite);
  }

  public void Move(string sourcePath, string destinationPath, bool overwrite)
  {
    if (overwrite)
      Delete(destinationPath, recursive: true);

    if (Directory.Exists(sourcePath))
      Directory.Move(sourcePath, destinationPath);
    else
      File.Move(sourcePath, destinationPath);
  }

  public void Delete(string path, bool recursive)
  {
    if (Directory.Exists(path))
      Directory.Delete(path, recursive);
    else if (File.Exists(path))
      File.Delete(path);
  }

  public string? ReadTextPreview(string path, int maxChars)
  {
    if (!File.Exists(path) || maxChars <= 0)
      return null;

    try
    {
      using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      var buffer = new byte[Math.Min(maxChars * 4, 8192)];
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
          info.LastWriteTime);

  private static void CopyDirectory(string sourcePath, string destinationPath, bool overwrite)
  {
    Directory.CreateDirectory(destinationPath);

    foreach (var file in Directory.EnumerateFiles(sourcePath))
    {
      var destinationFile = Path.Combine(destinationPath, Path.GetFileName(file));
      File.Copy(file, destinationFile, overwrite);
    }

    foreach (var directory in Directory.EnumerateDirectories(sourcePath))
    {
      var destinationDirectory = Path.Combine(destinationPath, Path.GetFileName(directory));
      CopyDirectory(directory, destinationDirectory, overwrite);
    }
  }
}

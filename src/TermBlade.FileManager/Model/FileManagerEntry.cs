namespace TermBlade.FileManager;

internal readonly record struct FileManagerEntry(
    string Name,
    string FullPath,
    bool IsDirectory,
    long Size,
    DateTimeOffset LastWriteTime)
{
  public static FileManagerEntry Directory(string path)
      => new(Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)), path, true, 0, DateTimeOffset.MinValue);

  public static FileManagerEntry File(string path, long size)
      => new(Path.GetFileName(path), path, false, size, DateTimeOffset.MinValue);
}

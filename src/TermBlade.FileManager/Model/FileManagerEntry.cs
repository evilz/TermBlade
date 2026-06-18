namespace TermBlade.FileManager;

internal readonly record struct FileManagerEntry(
    string Name,
    string FullPath,
    bool IsDirectory,
    long Size,
    DateTimeOffset LastWriteTime,
    string Permissions = "",
    string Owner = "",
    string Group = "")
{
  /// <summary>
  /// Directory.
  /// </summary>
  /// <param name="path">The path value.</param>
  public static FileManagerEntry Directory(string path)
      => new(Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)), path, true, 0, DateTimeOffset.MinValue);

  /// <summary>
  /// File.
  /// </summary>
  /// <param name="path">The path value.</param>
  /// <param name="size">The size value.</param>
  public static FileManagerEntry File(string path, long size)
      => new(Path.GetFileName(path), path, false, size, DateTimeOffset.MinValue);
}

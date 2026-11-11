using System;
using System.IO;

namespace TermBlade.FileManager;

/// <summary>
/// Compares paths using the names that the current filesystem resolves.
/// </summary>
internal static class FileSystemPathComparison
{
  public static bool PathsEqual(string path1, string path2)
      => string.Equals(Normalize(path1), Normalize(path2), StringComparison.Ordinal);

  public static bool IsSameOrDescendant(string parentPath, string candidatePath)
  {
    var parent = Normalize(parentPath);
    var candidate = Normalize(candidatePath);
    if (string.Equals(parent, candidate, StringComparison.Ordinal))
      return true;

    var prefix = parent.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
        ? parent
        : parent + Path.DirectorySeparatorChar;
    return candidate.StartsWith(prefix, StringComparison.Ordinal);
  }

  private static string Normalize(string path)
  {
    var fullPath = Path.GetFullPath(path);
    var root = Path.GetPathRoot(fullPath);
    if (string.IsNullOrEmpty(root))
      return fullPath;

    if (OperatingSystem.IsWindows())
      root = root.ToUpperInvariant();

    var currentPath = root;
    var relativePath = fullPath[root.Length..];
    var separators = new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
    foreach (var segment in relativePath.Split(separators, StringSplitOptions.RemoveEmptyEntries))
    {
      var resolvedName = ResolveExistingEntryName(currentPath, segment);
      currentPath = Path.Combine(currentPath, resolvedName ?? segment);
    }

    if (string.Equals(currentPath, root, StringComparison.Ordinal))
      return root;

    return currentPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
  }

  private static string? ResolveExistingEntryName(string parentPath, string requestedName)
  {
    try
    {
      string? caseInsensitiveMatch = null;
      foreach (var entryPath in Directory.EnumerateFileSystemEntries(parentPath))
      {
        var entryName = Path.GetFileName(entryPath);
        if (string.Equals(entryName, requestedName, StringComparison.Ordinal))
          return entryName;

        if (!string.Equals(entryName, requestedName, StringComparison.OrdinalIgnoreCase))
          continue;

        if (caseInsensitiveMatch is not null)
          return null;

        caseInsensitiveMatch = entryName;
      }

      if (caseInsensitiveMatch is null || !EntryExists(Path.Combine(parentPath, requestedName)))
        return null;

      return caseInsensitiveMatch;
    }
    catch (IOException)
    {
      return null;
    }
    catch (UnauthorizedAccessException)
    {
      return null;
    }
  }

  private static bool EntryExists(string path)
  {
    if (File.Exists(path) || Directory.Exists(path))
      return true;

    try
    {
      return (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0;
    }
    catch (IOException)
    {
      return false;
    }
    catch (UnauthorizedAccessException)
    {
      return false;
    }
  }
}

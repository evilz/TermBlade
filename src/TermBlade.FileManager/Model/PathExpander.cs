using System.Text.RegularExpressions;

namespace TermBlade.FileManager;

internal static partial class PathExpander
{
  /// <summary>
  /// Expand.
  /// </summary>
  /// <param name="path">The path value.</param>
  public static string Expand(string path)
  {
    var expanded = path;
    if (expanded == "~" || expanded.StartsWith("~/", StringComparison.Ordinal) || expanded.StartsWith(@"~\", StringComparison.Ordinal))
    {
      var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      expanded = home + expanded[1..];
    }

    return EnvironmentVariablePattern().Replace(expanded, match =>
        Environment.GetEnvironmentVariable(match.Groups[1].Value) ?? string.Empty);
  }

  [GeneratedRegex(@"\$\{([^}]+)\}")]
  private static partial Regex EnvironmentVariablePattern();
}

namespace TermBlade.Core.Ansi
{
  /// <summary>
  /// Records how the terminal should emit the color:
  /// literal RGB (38;2;r;g;b), indexed palette (38;5;n), or terminal default (39/49).
  /// </summary>
  public enum ColorIntent : byte
  {
    /// <summary>
    /// The rgb value.
    /// </summary>
    Rgb = 0,
    /// <summary>
    /// The indexed value.
    /// </summary>
    Indexed = 1,
    /// <summary>
    /// The default value.
    /// </summary>
    Default = 2,
  }
}

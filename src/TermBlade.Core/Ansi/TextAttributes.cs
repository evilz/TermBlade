namespace TermBlade.Core.Ansi;

/// <summary>Bit flags for standard ANSI text attributes.</summary>
[Flags]
public enum TextAttributes : uint
{
  /// <summary>
  /// The none value.
  /// </summary>
  None = 0,
  /// <summary>
  /// The bold value.
  /// </summary>
  Bold = 1 << 0,
  /// <summary>
  /// The dim value.
  /// </summary>
  Dim = 1 << 1,
  /// <summary>
  /// The italic value.
  /// </summary>
  Italic = 1 << 2,
  /// <summary>
  /// The underline value.
  /// </summary>
  Underline = 1 << 3,
  /// <summary>
  /// The blink value.
  /// </summary>
  Blink = 1 << 4,
  /// <summary>
  /// The inverse value.
  /// </summary>
  Inverse = 1 << 5,
  /// <summary>
  /// The hidden value.
  /// </summary>
  Hidden = 1 << 6,
  /// <summary>
  /// The strikethrough value.
  /// </summary>
  Strikethrough = 1 << 7,
}

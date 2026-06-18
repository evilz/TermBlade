using System;

namespace TermBlade.Core.Buffer
{
  /// <summary>
  /// Defines border style values.
  /// </summary>
  public enum BorderStyle { Single, Double, Rounded, Heavy }

  [Flags]
  /// <summary>
  /// Defines border sides values.
  /// </summary>
  public enum BorderSides { None = 0, Top = 1, Right = 2, Bottom = 4, Left = 8, All = 15 }

  /// <summary>
  /// Defines text alignment values.
  /// </summary>
  public enum TextAlignment { Left, Center, Right }

  /// <summary>
  /// Represents border chars.
  /// </summary>
  public static class BorderChars
  {
    // Index layout: 0=topLeft, 1=topRight, 2=bottomLeft, 3=bottomRight,
    //               4=horizontal, 5=vertical, 6=topT, 7=bottomT, 8=leftT, 9=rightT, 10=cross
    /// <summary>
    /// Gets the single.
    /// </summary>
    public static readonly int[] Single = { '┌', '┐', '└', '┘', '─', '│', '┬', '┴', '├', '┤', '┼' };
    /// <summary>
    /// Gets the double.
    /// </summary>
    public static readonly int[] Double = { '╔', '╗', '╚', '╝', '═', '║', '╦', '╩', '╠', '╣', '╬' };
    /// <summary>
    /// Gets the rounded.
    /// </summary>
    public static readonly int[] Rounded = { '╭', '╮', '╰', '╯', '─', '│', '┬', '┴', '├', '┤', '┼' };
    /// <summary>
    /// Gets the heavy.
    /// </summary>
    public static readonly int[] Heavy = { '┏', '┓', '┗', '┛', '━', '┃', '┳', '┻', '┣', '┫', '╋' };

    /// <summary>
    /// Get chars.
    /// </summary>
    /// <param name="style">The style value.</param>
    public static int[] GetChars(BorderStyle style) => style switch
    {
      BorderStyle.Double => Double,
      BorderStyle.Rounded => Rounded,
      BorderStyle.Heavy => Heavy,
      _ => Single,
    };
  }
}

using TermBlade.Core.Ansi;

namespace TermBlade.Core.Buffer
{
  /// <summary>A single terminal cell: codepoint + foreground/background colors + attributes.</summary>
  public struct Cell
  {
    /// <summary>
    /// Gets or sets the codepoint.
    /// </summary>
    public int Codepoint { get; set; }
    /// <summary>
    /// Gets or sets the fg.
    /// </summary>
    public Rgba Fg { get; set; }
    /// <summary>
    /// Gets or sets the bg.
    /// </summary>
    public Rgba Bg { get; set; }
    /// <summary>
    /// Gets or sets the attributes.
    /// </summary>
    public TextAttributes Attributes { get; set; }

    /// <summary>
    /// Empty.
    /// </summary>
    /// <param name="bg">The bg value.</param>
    public static Cell Empty(Rgba bg) => new Cell
    {
      Codepoint = ' ',
      Fg = Rgba.FromInts(255, 255, 255),
      Bg = bg,
      Attributes = TextAttributes.None,
    };
  }
}

using TermBlade.Core.Ansi;

namespace TermBlade.Core.Buffer
{
  /// <summary>A single terminal cell: codepoint + foreground/background colors + attributes.</summary>
  public struct Cell
  {
    public int Codepoint { get; set; }
    public Rgba Fg { get; set; }
    public Rgba Bg { get; set; }
    public TextAttributes Attributes { get; set; }

    public static Cell Empty(Rgba bg) => new Cell
    {
      Codepoint = ' ',
      Fg = Rgba.FromInts(255, 255, 255),
      Bg = bg,
      Attributes = TextAttributes.None,
    };
  }
}
